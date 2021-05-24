using Nov.Caps.Int.D365.Crm.Accounts;
using Nov.Caps.Int.D365.Crm.Businesses;
using Nov.Caps.Int.D365.Data.Abstract;
using Nov.Caps.Int.D365.Data.Queries;
using Nov.Caps.Int.D365.Models.Customers;
using Serilog;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Currencies;
using Nov.Caps.Int.D365.Crm.Systems;

namespace Nov.Caps.Int.D365.Services.Accounts
{
    public class SyncService
    {
        private readonly ILogger logger;
        private readonly ICustomerRepository customerRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IERPSystemRepository erpSystemRepository;

        private readonly ErpAccountService crmAccountsService;
        private readonly ErpSystemService crmErpSystemService;
        private readonly ErpParentSystemService crmErpParentSystemService;
        private readonly TransactionCurrencyService crmCurrencyService;
        private readonly BusinessUnitService crmBusinessUnitService;

        private readonly Dictionary<string, string> defaultCurrencyCodesByLedgers;

        public SyncService(
            ILogger logger,
            ICustomerRepository customerRepository,
            ICurrencyRepository currencyRepository,
            IERPSystemRepository erpSystemRepository,
            ErpAccountService crmAccountsService,
            ErpSystemService crmErpSystemService,
            ErpParentSystemService crmErpParentSystemService,
            TransactionCurrencyService crmCurrencyService,
            BusinessUnitService crmBusinessUnitService
        )
        {
            this.logger = logger.ForContext("service", "customer_accounts_sync");
            this.customerRepository = customerRepository;
            this.currencyRepository = currencyRepository;
            this.erpSystemRepository = erpSystemRepository;
            this.crmAccountsService = crmAccountsService;
            this.crmErpSystemService = crmErpSystemService;
            this.crmErpParentSystemService = crmErpParentSystemService;
            this.crmCurrencyService = crmCurrencyService;
            this.crmBusinessUnitService = crmBusinessUnitService;
            this.defaultCurrencyCodesByLedgers = new Dictionary<string, string>() {
                { "0197", "GBP" },
                { "1053", "USD" },
                { "0200", "USD" },
                { "0815", "USD" },
                { "0129", "USD" },
                { "0199", "USD" },
                { "0686", "USD" },
                { "0920", "SAR" },
                { "1008", "USD" },
                { "0233", "USD" },
                { "0432", "USD" },
                { "1109", "USD" },
                { "0433", "USD" },
                { "0434", "USD" },
                { "0435", "USD" }
            };
        }

        public async Task Sync()
        {
            var cancellation = new CancellationTokenSource();
            var failedCodes = new List<string>();
            var totalCounter = 0;
            var createdCounter = 0;
            var updatedCounter = 0;

            try
            {
                var ch = this.customerRepository.FindAsync(new CustomerFindFilter()
                {
                    LedgerNumber = this.defaultCurrencyCodesByLedgers.Keys.ToList(),
                    SystemID = new List<int>() { 478, 338, 414, 235, 62, 63, 65, 453, 549, 187, 354, 445 },
                }, cancellation.Token);

                while (await ch.WaitToReadAsync())
                {
                    var result = await ch.ReadAsync();

                    if (!result.IsOk)
                    {
                        throw result.Exception;
                    }

                    Customer customer = result.Data;
                    totalCounter++;

                    try
                    {
                        var created = await this.createOrUpdateAccount(customer);

                        if (created)
                        {
                            createdCounter++;

                            this.logger.Information("Successfully created a new customer account {code} {name}", customer.Code, customer.Name);
                        }
                        else
                        {
                            updatedCounter++;
                            this.logger.Information("Successfully updated a customer account {code} {name}", customer.Code, customer.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        failedCodes.Add(customer.Code);

                        this.logger.Error("Failed to sync customer account {@customer} {@error}", customer, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("Failed to sync customer accounts {@error} {total}", ex, totalCounter);

                throw ex;
            }
            finally
            {
                cancellation.Cancel();
            }


            if (failedCodes.Count == 0)
            {
                if ((createdCounter + updatedCounter) > 0)
                {
                    this.logger.Information("Successfully synced customer accounts {created} {updated} {total}", createdCounter, updatedCounter, totalCounter);
                }
                else
                {
                    this.logger.Information("Customer accounts are in sync {total}", totalCounter);
                }

                return;
            }

            if (failedCodes.Count <= 10)
            {
                var codes = string.Join(", ", failedCodes);

                this.logger.Information("Failed to sync the following customer accounts {codes} {created} {updated} {total}", codes, createdCounter, updatedCounter, totalCounter);

                throw new Exception($"Failed to sync the following customer accounts: {codes}");
            }

            this.logger.Information("Failed to sync customer accounts {failed} {created} {updated} {total}", failedCodes.Count, createdCounter, updatedCounter, totalCounter);

            throw new Exception($"Failed to sync {failedCodes.Count} of {totalCounter} customer account(s)");
        }

        private async Task<bool> createOrUpdateAccount(Customer customer)
        {
            var found = await this.crmAccountsService.FindBy(new ErpAccountFilter()
            {
                CustomerCode = customer.Code,
                SystemDescription = customer.SystemDescription,
            });

            // if a customer account already exists
            if (found.Any())
            {
                var account = found.First();

                //// Inactive
                //if (account.Status == 1) {
                //    return false;
                //}

                await this.updateAccount(account, customer);

                return false;
            }

            await this.createAccount(customer);

            return true;
        }

        private async Task updateAccount(ErpAccountEntity account, Customer customer) {

            await this.resolveAndMapFields(account, customer);
            await this.crmAccountsService.UpdateAsync(account);
        }

        private async Task createAccount(Customer customer)
        {
            var account = new ErpAccountEntity()
            {
                CapsID = null,
            };

            await this.resolveAndMapFields(account, customer);
            await this.crmAccountsService.CreateAsync(account);
        }

        private async Task resolveAndMapFields(ErpAccountEntity account, Customer customer)
        {
            account.CustomerName = customer.Name;
            account.CustomerCode = customer.Code;
            account.Street1 = customer.Address?.Line1;
            account.Street2 = customer.Address?.Line2;
            account.Street3 = customer.Address?.Line3;
            account.City = customer.Address?.City;
            account.State = customer.Address?.State;
            account.ZipCode = customer.Address?.Zip;
            account.LedgerNumber = customer.LedgerNumber;
            account.DefaultPaymentCode = customer.DefaultPaymentCode;
            account.DWCustomerID = customer.DWCustomerID;
            account.DWSource = Convert.ToInt32(customer.DWSource);
            account.CreditLimit = customer.CreditLimit > 0 ? customer.CreditLimit : 0;
            account.CreditLimitText = customer.CreditLimit.ToString();
            account.SystemID = customer.SystemID;
            account.SystemDescription = customer.SystemDescription;
            account.Telephone = customer.Telephone;

            // it's a new one
            if (!account.ID.HasValue)
            {
                account.StatusReason = 1;
            }

            System.Console.WriteLine($"Status is {account.Status}");

            var currency = await this.resolveCurrency(customer.CurrencyCode.Trim(), customer.LedgerNumber.Trim());

            if (currency != null)
            {                
                account.CurrencyID = currency.ID;
            }

            var erpSystem = await this.resolveERPSystem(customer.SystemID);

            if (erpSystem != null)
            {
                account.ERPSystemCodeID = erpSystem.ID;
            }
        }

        private async Task<TransactionCurrencyEntity> resolveCurrency(string currencyCode, string ledgerNumber) {
            if (String.IsNullOrWhiteSpace(currencyCode))
            {
                if (!this.defaultCurrencyCodesByLedgers.ContainsKey(ledgerNumber))
                {
                    // Return null and use value from CRM
                    return null;
                }

                currencyCode = this.defaultCurrencyCodesByLedgers[ledgerNumber];
            }


            var foundCRMCurrencies = await this.crmCurrencyService.FindByCodeAsync(currencyCode);

            if (foundCRMCurrencies.Any())
            {
                return foundCRMCurrencies.First();
            }

            var foundCurrencies = await this.currencyRepository.FindByCodes(new string[] { currencyCode });

            if (!foundCRMCurrencies.Any())
            {
                throw new Exception($"Unable to resolve currency: {currencyCode}");
            }

            var currency = foundCurrencies.First();
            var crmCurrency = new TransactionCurrencyEntity();
            crmCurrency.Code = currencyCode;
            crmCurrency.Name = currency.Code;
            crmCurrency.Precision = 1;
            crmCurrency.Rate = 1 / currency.Rate;
            crmCurrency.StatusReason = 1;
            crmCurrency.Symbol = "$";

            var currencyID = await this.crmCurrencyService.CreateAsync(crmCurrency);
            crmCurrency.ID = currencyID;

            return crmCurrency;
        }

        private async Task<ErpSystemEntity> resolveERPSystem(int id) {
            if (id <= 0)
            {
                return null;
            }

            var foundCRMERPSystems = await this.crmErpSystemService.FindByExternalIDAsync(id);

            if (foundCRMERPSystems.Any())
            {
                return foundCRMERPSystems.First();
            }

            var system = await this.erpSystemRepository.GetByIDAsync(id);

            if (system == null)
            {
                throw new Exception($"Could not find ERP system by id: {id}");
            }

            var erpSystem = new ErpSystemEntity();
            erpSystem.ActiveERP = system.ActiveERP.ToString();
            erpSystem.BusinessUnitDescription = system.BusinessUnitDescription;
            erpSystem.BusinessWK = system.BusinessWK.ToString();
            erpSystem.DefaultLedgerDescription = system.DefaultLedgerDescription;
            erpSystem.DefaultBusinessUnitID = system.DefaultBusinessUnitID.ToString();
            erpSystem.DefaultLedgerNumber = system.DefaultLedgerNumber;
            erpSystem.Name = system.Description;
            erpSystem.Description = system.Description;
            erpSystem.WK = system.WK.ToString();
            erpSystem.ServerName = string.Empty;

            if (!String.IsNullOrWhiteSpace(system.ParentSystem))
            {
                var foundParentSystems = await this.crmErpParentSystemService.FindByName(system.ParentSystem);

                if (foundParentSystems.Any())
                {
                    erpSystem.ParentID = foundParentSystems.First().ID;
                } else
                {
                    this.logger.Warning("Unable resolve parent system {systemid} {name} {parent}", id, erpSystem.Name, system.ParentSystem);
                }
            }

            if (!String.IsNullOrWhiteSpace(system.BusinessUnitCode))
            {
                var foundBusinessUnits = await this.crmBusinessUnitService.FindByNameAsync(system.BusinessUnitCode);

                if (foundBusinessUnits.Any())
                {
                    erpSystem.BusinessUnitCodeID = foundBusinessUnits.First().ID;
                }
                else
                {
                    this.logger.Warning("Unable resolve business unit {systemid} {business_unit}", id, system.BusinessUnitCode);
                }
            }

            var erpSystemID = await this.crmErpSystemService.CreateAsync(erpSystem);
            erpSystem.ID = erpSystemID;

            return erpSystem;
        }
    }
}
