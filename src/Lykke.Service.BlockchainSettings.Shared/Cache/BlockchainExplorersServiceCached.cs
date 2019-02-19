using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Caching.Distributed;
using Lykke.Service.BlockchainSettings.Core;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Shared.Cache.Interfaces;

namespace Lykke.Service.BlockchainSettings.Shared.Cache
{
    public class BlockchainExplorersServiceCached : IBlockchainExplorersServiceCached
    {
        private readonly IBlockchainExplorersService _blockchainExplorersService;
        private readonly IDistributedCache _distributedCache;
        private const string _explorersCacheKey = "ExplorersKey";

        public BlockchainExplorersServiceCached(IBlockchainExplorersService blockchainExplorersService,
            [KeyFilter(Constants.CacheServiceKey)]IDistributedCache distributedCache)
        {
            _blockchainExplorersService = blockchainExplorersService;
            _distributedCache = distributedCache;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BlockchainExplorer>> GetAllAsync()
        {
            var allBytes = await _distributedCache.GetAsync(_explorersCacheKey);

            IEnumerable<BlockchainExplorer> all = null;

            if (allBytes != null)
            {
                all = CacheSerializer.Deserialize<IEnumerable<BlockchainExplorer>>(allBytes);
            }
            else
            {
                all = await _blockchainExplorersService.GetAllAsync();
                await _distributedCache.SetAsync(_explorersCacheKey, CacheSerializer.Serialize(all), GetCacheOptions());
            }

            return all;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BlockchainExplorer>> GetAsync(string type)
        {
            var cacheKey = GetCacheKey(type);
            var allBytes = await _distributedCache.GetAsync(cacheKey);

            IEnumerable<BlockchainExplorer> all = null;

            if (allBytes != null)
            {
                all = CacheSerializer.Deserialize<IEnumerable<BlockchainExplorer>>(allBytes);
            }
            else
            {
                all = await _blockchainExplorersService.GetAsync(type);
                await _distributedCache.SetAsync(cacheKey, CacheSerializer.Serialize(all), GetCacheOptions());
            }

            return all;
        }

        public async Task<BlockchainExplorer> GetAsync(string type, string recordId)
        {
            var all = await GetAsync(type);

            return all?.FirstOrDefault(x => x.RecordId == recordId);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string type, string recordId)
        {
            await _blockchainExplorersService.RemoveAsync(type, recordId);
            await _distributedCache.RemoveAsync(_explorersCacheKey);
            await _distributedCache.RemoveAsync(GetCacheKey(type));
        }

        /// <inheritdoc/>
        public async Task CreateAsync(BlockchainExplorer settings)
        {
            await _blockchainExplorersService.CreateAsync(settings);
            await _distributedCache.RemoveAsync(_explorersCacheKey);
            await _distributedCache.RemoveAsync(GetCacheKey(settings.BlockchainType));
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(BlockchainExplorer settings)
        {
            await _blockchainExplorersService.UpdateAsync(settings);
            await _distributedCache.RemoveAsync(_explorersCacheKey);
            await _distributedCache.RemoveAsync(GetCacheKey(settings.BlockchainType));
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
            return $"ExplorersKey_{type}";
        }
    }
}
