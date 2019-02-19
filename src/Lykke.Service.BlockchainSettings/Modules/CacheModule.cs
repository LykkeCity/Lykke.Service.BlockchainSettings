using Autofac;
using Autofac.Features.AttributeFilters;
using JetBrains.Annotations;
using Lykke.Service.BlockchainSettings.Core;
using Lykke.Service.BlockchainSettings.Shared.Cache;
using Lykke.Service.BlockchainSettings.Shared.Cache.Interfaces;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;

namespace Lykke.Service.BlockchainSettings.Modules
{
    [UsedImplicitly]
    public class CacheModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public CacheModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var settingsCurrentValue = _settings.CurrentValue;
            IDistributedCache cache;

            if (settingsCurrentValue == null ||
                string.IsNullOrEmpty(settingsCurrentValue.BlockchainSettingsService.RedisCache.RedisConfiguration))
            {
                //InMemory
                var inMemoryCacheOptions = Options.Create(new MemoryDistributedCacheOptions());

                cache = new MemoryDistributedCache(inMemoryCacheOptions);
            }
            else
            {
                //Redis
                cache = new RedisCache(new RedisCacheOptions
                {
                    Configuration = _settings.CurrentValue.BlockchainSettingsService.RedisCache.RedisConfiguration,
                    InstanceName = _settings.CurrentValue.BlockchainSettingsService.RedisCache.InstanceName != null
                        ? $"BlockchainSettings:{_settings.CurrentValue.BlockchainSettingsService.RedisCache.InstanceName}:"
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

            builder.RegisterType<BlockchainExplorersServiceCached>()
                .As<IBlockchainExplorersServiceCached>()
                .SingleInstance()
                .WithAttributeFiltering();
        }
    }
}
