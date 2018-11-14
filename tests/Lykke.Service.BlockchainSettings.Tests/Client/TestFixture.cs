using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{
    /// <summary>
    /// A test fixture which hosts the target project (project we wish to test) in an in-memory server.
    /// </summary>
    public class TestFixture : IDisposable
    {
        private readonly TestServer _server;

        public TestFixture()
        {
#if DEBUG
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.tests.json", true)
                .Build();
            
            if (!string.IsNullOrEmpty(config["SettingsUrl"]))
                Environment.SetEnvironmentVariable("SettingsUrl", config["SettingsUrl"]);
#endif
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<TestStartup>();

            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri(ClientUrl);
        }

        public HttpClient Client { get; }

        public string ClientUrl => "http://localhost";

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}
