using Microsoft.Data.SqlClient;

namespace CIC.DataUtility
{
    public static class GetDBConnection
    {
        public static SqlConnection getConnection(string configuration)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = configuration;
            return sqlConnection;
        }
    }
}
