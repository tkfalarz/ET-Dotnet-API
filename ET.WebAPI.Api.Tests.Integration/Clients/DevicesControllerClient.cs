using ET.WebAPI.Api.Tests.Integration.Auth;
using ET.WebAPI.Api.Views;
using ET.WebAPI.Database;
using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.TestsUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Integration.Clients
{
    public class DevicesControllerClient : WebApplicationFactory<Startup>
    {
        private const string BaseUrl = "api/Devices";
        public HttpClient HttpClient { get; private set; }
        public ApiDbContext DbContext { get; private set; }

        public DevicesControllerClient()
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

        public async Task<HttpResponseMessage> StoreDeviceAsync(DeviceView content)
            => await HttpClient.PostAsync(new Uri(BaseUrl, UriKind.Relative), JsonContent.Create(content));

        public async Task<HttpResponseMessage> GetDevicesAsync() => await HttpClient.GetAsync("api/Devices");

        public async Task<HttpResponseMessage> GetDeviceAsync(string deviceName) => await HttpClient.GetAsync($"api/Devices/{deviceName}");

        public async Task<HttpResponseMessage> GetDeviceReadingsAsync(string deviceName)
            => await HttpClient.GetAsync($"api/Devices/{deviceName}/Readings");

        public async Task<HttpResponseMessage> GetDeviceLatestReadingsAsync(string deviceName)
            => await HttpClient.GetAsync($"api/Devices/{deviceName}/Readings/Latest");

        public async Task<HttpResponseMessage> GetDeviceWeatherFactorReadingsAsync(string deviceName, ReadingType readingType)
            => await HttpClient.GetAsync($"api/Devices/{deviceName}/Readings/{readingType}");

        public async Task<HttpResponseMessage> GetDeviceLatestWeatherFactorReadingsAsync(string deviceName, ReadingType readingType)
            => await HttpClient.GetAsync($"api/Devices/{deviceName}/Readings/{readingType}/Latest");

        private static string ConnectionString 
            => TestDbConnectionStringProvider.GetConnectionString($"{nameof(DevicesControllerClient)}-WebApiTests");
    }
}