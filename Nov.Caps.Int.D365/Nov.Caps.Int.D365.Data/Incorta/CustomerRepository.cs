using Dapper;
using Nov.Caps.Int.D365.Common;
using Nov.Caps.Int.D365.Data.Abstract;
using Nov.Caps.Int.D365.Data.Common;
using Nov.Caps.Int.D365.Data.Queries;
using Nov.Caps.Int.D365.Models.Common;
using Nov.Caps.Int.D365.Models.Customers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Data.Incorta
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ConnectionSettings settings;

        public CustomerRepository(ConnectionSettings settings)
        {
            this.settings = settings;
        }

        public ChannelReader<Result<Customer>> FindAsync(CustomerFindFilter filter, CancellationToken cancellation)
        {
            var ch = Channel.CreateBounded<Result<Customer>>(1);

            Task.Run(async () => {
                try
                {
                    using (var conn = new NpgsqlConnection(this.settings.ToString()))
                    {
                        await conn.OpenAsync();

                        var sb = new StringBuilder();
                        sb.AppendLine(@"
                        SELECT
                            a.CUSTOMER_CODE as Code,
                            a.CUSTOMER_NAME as Name,
                            a.CUSTOMER_TELEPHONE as Telephone,
                            a.LEDGER_NUM as LedgerNumber,
                            a.DEFAULT_PAYMENT_CODE as DefaultPaymentCode,
                            a.DW_CUSTOMER_ID as DWCustomerID,
                            a.DW_SOURCE as DWSource,
                            a.SYSTEM_ID as SystemID,
                            a.SYSTEM_DESC as SystemDescription,
                            a.CREDIT_LIMIT as CreditLimit,
                            'USD' as CurrencyCode,
                            a.CUSTOMER_COUNTRY as Country,
                            a.CUSTOMER_STATE as State,
                            a.CUSTOMER_CITY as City,
                            a.CUSTOMER_ADDRESS_1 as Line1,
                            a.CUSTOMER_ADDRESS_2 as Line2,
                            a.CUSTOMER_ZIP as Zip
                        FROM UDS_CAPS_CX.D_Customer_Master a"
                    );

                        var filterValues = new List<KeyValuePair<string, List<object>>>();

                        if (filter.LedgerNumber != null)
                        {
                            filterValues.Add(new KeyValuePair<string, List<object>>("a.LEDGER_NUM", new List<object>(filter.LedgerNumber)));
                        }

                        if (filter.SystemID != null)
                        {
                            filterValues.Add(new KeyValuePair<string, List<object>>("a.SYSTEM_ID", new List<object>(filter.SystemID.ConvertAll<object>(i => i))));
                        }

                        if (filter.CustomerCode != null)
                        {
                            filterValues.Add(new KeyValuePair<string, List<object>>("a.CUSTOMER_CODE", new List<object>(filter.CustomerCode)));
                        }

                        var tpl = QueryBuilder.BuildWhere(filterValues);

                        if (!string.IsNullOrWhiteSpace(tpl.Query))
                        {
                            sb.AppendLine("WHERE");
                            sb.AppendLine(tpl.Query);
                        }

                        var result = conn.Query<Customer, Address, Customer>(sb.ToString(), (customer, address) =>
                        {
                            customer.Address = address;

                            return customer;
                        }, tpl.Parameters, splitOn: "Country", buffered: false);

                        foreach (var customer in result)
                        {
                            if (cancellation.IsCancellationRequested)
                            {
                                break;
                            }

                            await ch.Writer.WaitToWriteAsync();
                            await ch.Writer.WriteAsync(new Result<Customer>(customer));
                        }
                    }
                }
                catch (Exception ex)
                {
                    await ch.Writer.WaitToWriteAsync();
                    await ch.Writer.WriteAsync(new Result<Customer>(ex));
                }
                finally
                {
                    ch.Writer.Complete();
                }
            }, cancellation);

            return ch.Reader;
        }
    }
}
