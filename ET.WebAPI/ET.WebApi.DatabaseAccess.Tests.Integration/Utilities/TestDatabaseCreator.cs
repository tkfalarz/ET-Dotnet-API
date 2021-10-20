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
                ConnectionString = new ConfigurationBuilder()
                    .AddJsonFile($"testSettings.json", optional: false)
                    .Build()
                    .GetConnectionString("ApiDb"),
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

        public void Dispose()
        {
            Context?.Database?.EnsureDeletedAsync();
        }
    }
}