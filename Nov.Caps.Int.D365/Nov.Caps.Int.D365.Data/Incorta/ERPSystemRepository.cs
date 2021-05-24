using System.Data;
using System.Threading.Tasks;
using Dapper;
using Nov.Caps.Int.D365.Data.Abstract;
using Nov.Caps.Int.D365.Data.Common;
using Nov.Caps.Int.D365.Models.ERPSystems;
using Npgsql;

namespace Nov.Caps.Int.D365.Data.Incorta
{
    public class ERPSystemRepository : IERPSystemRepository
    {
        private static string selectFields = @"
            ERP_SYSTEM_WK as Wk,
            SYSTEM_ID as ID,
            SYSTEM_DESC as Description,
            PARENT_SYSTEM as ParentSystem,
            DEFAULT_BUSINESS_UNIT_ID as DefaultBusinessUnitID,
            BUSINESS_WK as BusinessWK,
            DEFAULT_LEDGER_NUMBER as DefaultLedgerNumber,
            DEFAULT_LEDGER_DESCRIPTION as DefaultLedgerDescription,
            ACTIVE_ERP as ActiveERP,
            BUSINESS_NAME as BusinessName,
            BUSINESS_UNIT_CODE as BusinessUnitCode,
            BUSINESS_UNIT_DESCRIPTION as BusinessUnitDescription, 
            IT_CONTACT as ITContact,
            NOTES as Notes
        ";

        private readonly ConnectionSettings settings;

        public ERPSystemRepository(ConnectionSettings settings)
        {
            this.settings = settings;
        }

        public async Task<ERPSystem> GetByIDAsync(int id)
        {
            using (var conn = new NpgsqlConnection(this.settings.ToString()))
            {
                await conn.OpenAsync();

                var query = @$"
                        SELECT
                           {ERPSystemRepository.selectFields}
                        FROM EXTENSION_CAPS_S.TBL_DIM_ERP_SYSTEM
                        WHERE
                            SYSTEM_ID = @id
                        LIMIT 1";

                var parameter = new DynamicParameters();
                parameter.Add("@id", id, DbType.UInt16, ParameterDirection.Input);

                var result = await conn.QuerySingleAsync<ERPSystem>(query, parameter);

                return result;
            }
        }
    }
}
