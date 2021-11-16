using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;

namespace ET.WebAPI.TestsUtilities
{
    public class TestDbConnectionStringProvider
    {
        public static string GetConnectionString(string initialCatalogName)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
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
                connectionString = new DbConnectionStringBuilder
                {
                    ["Data Source"] = "localhost",
                    ["Integrated Security"] = "True",
                    ["Connect Timeout"] = "30",
                    ["Encrypt"] = "False",
                    ["TrustServerCertificate"] = "False",
                    ["ApplicationIntent"] = "ReadWrite",
                    ["MultiSubnetFailover"] = "False"
                }.ConnectionString;

            return connectionString;
        }
    }
}