using Dapper;
using Nov.Caps.Int.D365.Data.Abstract;
using Nov.Caps.Int.D365.Data.Common;
using Nov.Caps.Int.D365.Data.Common.Helpers;
using Nov.Caps.Int.D365.Models.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Data.Incorta
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ConnectionSettings settings;

        public CurrencyRepository(ConnectionSettings settings)
        {
            this.settings = settings;
        }

        public Task<IEnumerable<Currency>> FindByCodes(string[] codes)
        {
            return this.FindByCodes(codes, DateTime.Now);
        }

        public async Task<IEnumerable<Currency>> FindByCodes(string[] codes, DateTime dateTime)
        {
            using (var conn = new NpgsqlConnection(this.settings.ToString()))
            {
                await conn.OpenAsync();

                var query = @"
                        SELECT
                            CURRENCY as code,
                            EXCHANGE_RATE_TO_USD as rate
                        FROM EXTENSION_CAPS_S.TBL_FACT_EXCHANGE_RATES
                        WHERE
                            TIME_ID = @timeid AND LOWER(CURRENCY) IN (@codes)";

                var parameter = new DynamicParameters();
                parameter.Add("@timeid", DateTimeHelper.ToTimeID(DateTime.Now), DbType.String, ParameterDirection.Input);
                parameter.Add("@codes", string.Join("', '", codes).ToLowerInvariant());

                var result = conn.Query<Currency>(query, parameter);

                return result.AsList();
            }
        }
    }
}
