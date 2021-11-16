using ET.WebAPI.Database;
using ET.WebAPI.TestsUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ET.WebApi.Database.Tests.Integration.Utilities
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

            builder.UseSqlServer(TestDbConnectionStringProvider.GetConnectionString(databaseInitialCatalogName));
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
    }
}