using System.Configuration;
using Npgsql;
using System.Data;

namespace gizindir.data
{
    public static class DbContext
    {
        public static NpgsqlConnection GetConnection()
        {
            string connString = ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString;
            return new NpgsqlConnection(connString);
        }
    }
}
