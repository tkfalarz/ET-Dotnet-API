using ET.WebAPI.DatabaseAccess.DatabaseSetup;
using ET.WebApi.DatabaseAccess.Tests.Integration.Utilities;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ET.WebApi.DatabaseAccess.Tests.Integration
{
    [SetUpFixture]
    public class DatabaseAccessSetUpFixture
    {
        private ApiDbContext context;

        public DatabaseAccessSetUpFixture()
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
            // await Database.DisposeAsync();
        }
    }
}