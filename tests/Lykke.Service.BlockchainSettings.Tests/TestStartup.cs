using System;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.BlockchainSettings.Modules;
using Lykke.Service.BlockchainSettings.Shared.Attributes;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.Service.BlockchainSettings.Shared.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainSettings.Tests
{
    //Used in hosted server
    public class TestStartup
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
                    logs.UseEmptyLogging();
                };
                
                options.RegisterAdditionalModules = x =>
                {
                    x.RegisterModule<ServiceModule>();
                    x.RegisterModule<CacheModule>();
                    x.RegisterModule<DataLayerModule>();
                    x.RegisterModule<SecurityModule>();
                    x.RegisterModule<MocksModule>();
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
