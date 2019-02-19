using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Caching.Distributed;
using Lykke.Service.BlockchainSettings.Core;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Shared.Cache.Interfaces;

namespace Lykke.Service.BlockchainSettings.Shared.Cache
{
    public class BlockchainSettingsServiceCached : IBlockchainSettingsServiceCached
    {
        private readonly IBlockchainSettingsService _blockchainSettingsService;
        private readonly IDistributedCache _distributedCache;
        private const string _settingsCacheKey = "SettingsKey";

        public BlockchainSettingsServiceCached(IBlockchainSettingsService blockchainSettingsService,
            [KeyFilter(Constants.CacheServiceKey)]IDistributedCache distributedCache)
        {
            _blockchainSettingsService = blockchainSettingsService;
            _distributedCache = distributedCache;
        }

        /// <inheritdoc/>
        public async Task CreateAsync(BlockchainSetting settings)
        {
            await _blockchainSettingsService.CreateAsync(settings);
            await _distributedCache.RemoveAsync(_settingsCacheKey);
            await _distributedCache.RemoveAsync(GetCacheKey(settings.Type));
        }

        public async Task<IEnumerable<BlockchainSetting>> GetAllAsync()
        {
            var allBytes = await _distributedCache.GetAsync(_settingsCacheKey);

            IEnumerable<BlockchainSetting> all = null;

            if (allBytes != null)
            {
                all = CacheSerializer.Deserialize<IEnumerable<BlockchainSetting>>(allBytes);
            }
            else
            {
                all = await _blockchainSettingsService.GetAllAsync();
                await _distributedCache.SetAsync(_settingsCacheKey, CacheSerializer.Serialize(all), GetCacheOptions());
            }

            return all;
        }

        public async Task<(IEnumerable<BlockchainSetting>, string continuationToken)> GetAllAsync(int take, string continuationToken = null)
        {
            var (collection, token) = await _blockchainSettingsService.GetAllAsync(take, continuationToken);

            return (collection, token);
        }

        public async Task<BlockchainSetting> GetAsync(string type)
        {
            var cachedBytes = await _distributedCache.GetAsync(GetCacheKey(type));
            BlockchainSetting item = null;

            if (cachedBytes != null)
            {
                item = CacheSerializer.Deserialize<BlockchainSetting>(cachedBytes);
            }
            else
            {
                item = await _blockchainSettingsService.GetAsync(type);

                await _distributedCache.SetAsync(GetCacheKey(type), CacheSerializer.Serialize(item), GetCacheOptions());
            }

            return item;
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string type)
        {
            await _blockchainSettingsService.RemoveAsync(type);
            await _distributedCache.RemoveAsync(GetCacheKey(type));
            await _distributedCache.RemoveAsync(_settingsCacheKey);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(BlockchainSetting settings)
        {
            await _blockchainSettingsService.UpdateAsync(settings);
            await _distributedCache.RemoveAsync(GetCacheKey(settings.Type));
            await _distributedCache.RemoveAsync(_settingsCacheKey);
        }

        private DistributedCacheEntryOptions GetCacheOptions()
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
            };

            return options;
        }

        private string GetCacheKey(string type)
        {
            return $"SettingsKey_{type}";
        }
    }
}
