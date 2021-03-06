using ET.WebAPI.Api.Tests.Integration.Auth;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Database;
using ET.WebAPI.TestsUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Integration.Clients
{
    public class ReadingsControllerClient : WebApplicationFactory<Startup>
    {
        private const string ReadingsBaseUrl = "api/Readings";
        public HttpClient HttpClient { get; private set; }
        public ApiDbContext DbContext { get; private set; }

        public ReadingsControllerClient()
        {
            HttpClient = new WebApplicationFactory<Startup>().WithWebHostBuilder(
                x =>
                {
                    x.ConfigureTestServices(
                        services =>
                        {
                            services
                                .AddAuthorization(
                                    x =>
                                    {
                                        x.DefaultPolicy = new AuthorizationPolicyBuilder()
                                            .AddRequirements(new TestAuthorizationRequirement())
                                            .Build();
                                    });
                            DbContext = services.BuildServiceProvider().GetRequiredService<ApiDbContext>();
                            DbContext.Database.Migrate();
                        });
                    x.UseSetting("SQL_CONNECTION_STRING", ConnectionString);
                })
                .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        }

        public async Task<HttpResponseMessage> StoreReadingAsync(ReadingSetView content) 
            => await HttpClient.PostAsync(ReadingsBaseUrl, JsonContent.Create(content));

        public async Task<HttpResponseMessage> GetNearestLatestReadingsAsync(decimal latitude, decimal longitude) 
            => await HttpClient.GetAsync($"api/Readings/nearest?latitude={latitude}&longitude={longitude}");

        private static string ConnectionString => TestDbConnectionStringProvider.GetConnectionString($"{nameof(ReadingsControllerClient)}-WebApiTests");
    }
}