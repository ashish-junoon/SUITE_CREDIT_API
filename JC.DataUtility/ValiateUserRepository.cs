using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIC.DataUtility
{
    public class ValiateUserRepository
    {
        public static ExternalPartner? GetPartnersDetails(string token, string vendor, string action_name, string dbconnection, ILoggerManager _logger)
        {
            ExternalPartner? partner = new ExternalPartner();
            DataSet? ds = null;
            try
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Token", SqlDbType.VarChar) { Value = token },
                    new SqlParameter("@VendorCode", SqlDbType.VarChar) { Value = vendor },
                    new SqlParameter("@ServiceName", SqlDbType.VarChar) { Value = action_name }
                };
                using (SqlConnection con = GetDBConnection.getConnection(dbconnection))
                {
                    ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Usp_GetPartnersDetails_V1", param);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        DataRow row = dt.Rows[0];
                        partner.message = row["message"] != DBNull.Value ? Convert.ToString(row["message"]) : string.Empty;
                        partner.status = row["status"] != DBNull.Value && Convert.ToBoolean(row["status"]);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error in GetPartnersDetails method: {ex.Message}");
            }
            return partner;
        }
    }
}
