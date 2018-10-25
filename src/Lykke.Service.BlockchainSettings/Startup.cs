using Lykke.Service.BlockchainSettings.Shared.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using JetBrains.Annotations;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Sdk;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.Service.BlockchainSettings.Shared.Swagger;

namespace Lykke.Service.BlockchainSettings
{
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "BlockchainSettings API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Swagger = genOptions =>
                {
                    genOptions.OperationFilter<ApiKeyHeaderAccessTokenOperationFilter>();
                };

                options.ConfigureMvcOptions = mvcOptions =>
                {
                    mvcOptions.Filters.Add(typeof(CheckModelStateAttribute), 0);
                    mvcOptions.Filters.Add(typeof(ValidateActionParametersAttribute), 1);
                };

                options.Logs = logs =>
                {
                    logs.AzureTableName = "BlockchainSettingsLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.BlockchainSettingsService.Db.LogsConnectionString;
                    
                    logs.Extended = extendedLogs =>
                    {
                        extendedLogs.AddAdditionalSlackChannel("CommonBlockChainIntegration", channelOptions =>
                        {
                            channelOptions.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                        });
                        
                        extendedLogs.AddAdditionalSlackChannel("CommonBlockChainIntegrationImportantMessages", channelOptions =>
                        {
                            channelOptions.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
                        });
                    };
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }
    }
}
