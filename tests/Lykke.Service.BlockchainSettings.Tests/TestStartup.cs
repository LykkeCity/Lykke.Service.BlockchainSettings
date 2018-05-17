using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.MonitoringServiceApiCaller;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Shared.Attributes;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.Service.BlockchainSettings.Shared.Swagger;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSettings.Shared;

namespace Lykke.Service.BlockchainSettings
{
    //Used in hosted server
    public class TestStartup : BlockchainSettingsStartupBase
    {
        private string _monitoringServiceUrl;

        public TestStartup(IHostingEnvironment env) : base(env)
        {
        }

        public override (IContainer, ILog) RegisterContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            var log = new LogToConsole();

            builder.Populate(services);

            return (builder.Build(), Log);
        }

        protected override async Task StartApplication()
        {
        }

        protected override async Task StopApplication()
        {
        }

        protected override async Task CleanUp()
        {
        }
    }
}
