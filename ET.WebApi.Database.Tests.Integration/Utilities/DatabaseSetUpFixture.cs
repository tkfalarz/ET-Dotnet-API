using ET.WebAPI.Database;
using ET.WebApi.Database.Tests.Integration.Utilities;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ET.WebApi.Database.Tests.Integration
{
    [SetUpFixture]
    public class DatabaseSetUpFixture
    {
        private ApiDbContext context;

        public DatabaseSetUpFixture()
        {
        }

        [OneTimeSetUp]
        public void BeforeAll()
        {
            context = TestDatabaseCreator.CreateTestDatabaseContext();
        }

        [OneTimeTearDown]
        public async Task AfterAll()
        {
            await context?.Database?.EnsureDeletedAsync();
        }
    }
}