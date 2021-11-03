using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;

namespace ET.WebAPI.TestsUtilities
{
    public class TestDbConnectionStringProvider
    {
        public static string GetConnectionString(string initialCatalogName)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                ConnectionString = GetConnectionString(),
                InitialCatalog = initialCatalogName
            };
            return connectionStringBuilder.ConnectionString;
        }

        private static string GetConnectionString()
        {
            var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = new DbConnectionStringBuilder()
                {
                    ["Data Source"] = (object)"localhost",
                    ["Integrated Security"] = (object)"True",
                    ["Connect Timeout"] = (object)"30",
                    ["Encrypt"] = (object)"False",
                    ["TrustServerCertificate"] = (object)"False",
                    ["ApplicationIntent"] = (object)"ReadWrite",
                    ["MultiSubnetFailover"] = (object)"False"
                }.ConnectionString;

            return connectionString;
        }
    }
}