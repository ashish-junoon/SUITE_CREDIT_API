using Microsoft.Data.SqlClient;
using System.Data;

namespace CIC.DataUtility.Repository
{
    public class CommonRepository
    {
        public static ExternalPartner? GetPartnersDetails(string token, string vendor, string action_name, string dbconnection)
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
                        partner.message = row["Message"] != DBNull.Value ? Convert.ToString(row["Message"]) : string.Empty;
                        partner.status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]);
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger?.LogError($"Error in GetPartnersDetails method: {ex.Message}");
            }
            return partner;
        }
    }
}
