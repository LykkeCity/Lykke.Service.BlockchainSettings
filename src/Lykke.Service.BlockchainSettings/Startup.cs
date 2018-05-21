using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.MonitoringServiceApiCaller;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Modules;
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
    public class Startup : BlockchainSettingsStartupBase
    {
        private string _monitoringServiceUrl;

        public Startup(IHostingEnvironment env) : base(env)
        {
        }

        public override (IContainer, ILog) RegisterContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            var appSettings = Configuration.LoadSettings<AppSettings>();
            _monitoringServiceUrl = appSettings.CurrentValue.MonitoringServiceClient.MonitoringServiceUrl;

            var log = CreateLogWithSlack(services, appSettings);

            builder.RegisterModule(new ServiceModule(appSettings.Nested(x => x.BlockchainSettingsService), log));
            builder.RegisterModule(new DataLayerModule(appSettings.Nested(x => x.BlockchainSettingsService), log));
            builder.RegisterModule(new CacheModule(appSettings.Nested(x => x.BlockchainSettingsService.RedisCache), log));
            builder.RegisterModule(new SecurityModule(appSettings.Nested(x => x.ApiKeys), log));
            builder.Populate(services);

            return (builder.Build(), log);
        }

        protected override async Task StartApplication()
        {
            try
            {
                // NOTE: Service not yet recieve and process requests here

                await ApplicationContainer.Resolve<IStartupManager>().StartAsync();

                await Log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Started");

                await AutoRegistrationInMonitoring.RegisterAsync(Configuration, _monitoringServiceUrl, Log);
            }
            catch (Exception ex)
            {
                await Log.WriteFatalErrorAsync(nameof(Startup), nameof(StartApplication), "", ex);
                throw;
            }
        }

        protected override async Task StopApplication()
        {
            try
            {
                // NOTE: Service still can recieve and process requests here, so take care about it if you add logic here.

                await ApplicationContainer.Resolve<IShutdownManager>().StopAsync();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(Startup), nameof(StopApplication), "", ex);
                }
                throw;
            }
        }

        protected override async Task CleanUp()
        {
            try
            {
                // NOTE: Service can't recieve and process requests here, so you can destroy all resources

                if (Log != null)
                {
                    await Log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Terminating");
                }

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(Startup), nameof(CleanUp), "", ex);
                    (Log as IDisposable)?.Dispose();
                }
                throw;
            }
        }

        private static ILog CreateLogWithSlack(IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            var dbLogConnectionStringManager = settings.Nested(x => x.BlockchainSettingsService.Db.LogsConnectionString);
            var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

            if (string.IsNullOrEmpty(dbLogConnectionString))
            {
                consoleLogger.WriteWarningAsync(nameof(Startup), nameof(CreateLogWithSlack), "Table loggger is not inited").Wait();
                return aggregateLogger;
            }

            if (dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}"))
                throw new InvalidOperationException($"LogsConnString {dbLogConnectionString} is not filled in settings");

            var persistenceManager = new LykkeLogToAzureStoragePersistenceManager(
                AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, "BlockchainSettingsLog", consoleLogger),
                consoleLogger);

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueIntegration.AzureQueueSettings
            {
                ConnectionString = settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.CurrentValue.SlackNotifications.AzureQueue.QueueName
            }, aggregateLogger);

            var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);

            // Creating azure storage logger, which logs own messages to concole log
            var azureStorageLogger = new LykkeLogToAzureStorage(
                persistenceManager,
                slackNotificationsManager,
                consoleLogger);

            azureStorageLogger.Start();

            aggregateLogger.AddLog(azureStorageLogger);

            return aggregateLogger;
        }
    }
}
