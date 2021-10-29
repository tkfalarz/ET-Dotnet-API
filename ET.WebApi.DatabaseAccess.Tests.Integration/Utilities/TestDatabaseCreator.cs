using ET.WebAPI.DatabaseAccess.DatabaseSetup;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.IO;
using System.Xml.Linq;

namespace ET.WebApi.DatabaseAccess.Tests.Integration.Utilities
{
    public sealed class TestDatabaseCreator : IDisposable
    {
        public ApiDbContext Context { get; private set; }
        private static TestDatabaseCreator creator;

        private TestDatabaseCreator(string databaseInitialCatalogName)
        {
            var serviceCollection = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder();
            
            var connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                ConnectionString = GetConnectionString(),
                InitialCatalog = databaseInitialCatalogName
            };

            builder.UseSqlServer(connectionStringBuilder.ConnectionString);
            Context = new ApiDbContext(builder.Options);
            Context.Database.Migrate();
        }

        public static ApiDbContext CreateTestDatabaseContext()
        {
            if (creator == null)
                creator = new TestDatabaseCreator("ET_WebAPI_Integration_Tests");
            return creator.Context;
        }

        public void Dispose() => Context?.Database?.EnsureDeletedAsync();

        private string GetConnectionString()
        {
            var connectionString = Environment.GetEnvironmentVariable("GITHUB_WORKFLOW_CONNECTION_STRING");
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