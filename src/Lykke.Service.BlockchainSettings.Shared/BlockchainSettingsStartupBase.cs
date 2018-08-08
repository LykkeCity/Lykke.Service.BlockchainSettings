using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.MonitoringServiceApiCaller;
using Lykke.Service.BlockchainSettings.Shared.Attributes;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.Service.BlockchainSettings.Shared.Swagger;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainSettings.Shared
{
    public abstract class BlockchainSettingsStartupBase
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; protected set; }
        public IConfigurationRoot Configuration { get; }
        public ILog Log { get; private set; }

        public BlockchainSettingsStartupBase(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc(options =>
                    {
                        options.Filters.Add(typeof(CheckModelStateAttribute), 0);
                        options.Filters.Add(typeof(ValidateActionParametersAttribute), 1);
                    })
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver =
                            new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    });
                
                services.AddSwaggerGen(options =>
                {
                    options.DefaultLykkeConfiguration("v1", "BlockchainSettings API");

                    options.OperationFilter<ApiKeyHeaderAccessTokenOperationFilter>();
                });

                (ApplicationContainer, Log) = RegisterContainer(services);

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                Log?.WriteFatalError(this.GetType().Name, nameof(ConfigureServices), ex);
                throw;
            }
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseLykkeForwardedHeaders();
                app.UseLykkeMiddleware("BlockchainSettings", ex => new { Message = "Technical problem" });

                app.UseMvc();
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
                });
                app.UseSwaggerUI(x =>
                {
                    x.RoutePrefix = "swagger/ui";
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
                app.UseStaticFiles();

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopping.Register(() => StopApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(() => CleanUp().GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                Log?.WriteFatalError(this.GetType().Name, nameof(Configure), ex);
                throw;
            }
        }

        public abstract (IContainer, ILog) RegisterContainer(IServiceCollection collection);

        protected abstract Task StartApplication();

        protected abstract Task StopApplication();

        protected abstract Task CleanUp();
    }
}
