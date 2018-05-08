using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BlockchainSettings.AzureRepositories.Repositories;
using Lykke.Service.BlockchainSettings.Cache;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using Lykke.Service.BlockchainSettings.Settings.ServiceSettings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainSettings.Modules
{
    public class CacheModule : Module
    {
        private readonly IReloadingManager<CacheSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public CacheModule(IReloadingManager<CacheSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            var redis = new RedisCache(new RedisCacheOptions
            {
                Configuration = _settings.CurrentValue.RedisConfiguration,
                InstanceName = _settings.CurrentValue.InstanceName
            });

            builder.RegisterInstance(redis)
                .As<IDistributedCache>()
                .SingleInstance();

            builder.RegisterType<BlockchainSettingsServiceCached>()
                .As<IBlockchainSettingsServiceCached>()
                .SingleInstance();

            builder.Populate(_services);
        }
    }
}
