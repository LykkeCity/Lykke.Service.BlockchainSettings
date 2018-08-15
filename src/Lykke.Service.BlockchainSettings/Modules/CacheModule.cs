using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Common.Log;
using Lykke.Service.BlockchainSettings.AzureRepositories.Repositories;
using Lykke.Service.BlockchainSettings.Core;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using Lykke.Service.BlockchainSettings.Shared.Cache;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            var settingsCurrentValue = _settings.CurrentValue;
            IDistributedCache cache = null;

            if (settingsCurrentValue == null ||
                string.IsNullOrEmpty(settingsCurrentValue.InstanceName) ||
                string.IsNullOrEmpty(settingsCurrentValue.RedisConfiguration))
            {
                //InMemory
                var inMemoryCacheOptions = Options.Create(new MemoryDistributedCacheOptions()
                {
                });

                cache = new MemoryDistributedCache(inMemoryCacheOptions);
            }
            else
            {
                //Redis
                cache = new RedisCache(new RedisCacheOptions
                {
                    Configuration = _settings.CurrentValue.RedisConfiguration,
                    InstanceName = _settings.CurrentValue.InstanceName != null
                        ? $"BlockchainSettings:{_settings.CurrentValue.InstanceName}:"
                        : "BlockchainSettings:"
                });
            }

            builder.RegisterInstance(cache)
                .As<IDistributedCache>()
                .Keyed<IDistributedCache>(Constants.CacheServiceKey)
                .SingleInstance();

            builder.RegisterType<BlockchainSettingsServiceCached>()
                .As<IBlockchainSettingsServiceCached>()
                .SingleInstance()
                .WithAttributeFiltering();

            builder.Populate(_services);
        }
    }
}
