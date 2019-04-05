using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Tests.Fakes
{
    public class DistributedCacheFake : IDistributedCache
    {
        public Dictionary<string, byte[]> MemoryCacheDict = new Dictionary<string, byte[]>();

        public byte[] Get(string key)
        {
            if (!MemoryCacheDict.TryGetValue(key, out var result))
                return null;

            return result;
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(Get(key));
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            MemoryCacheDict[key] = value;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            Set(key, value, options);

            return Task.FromResult(0);
        }

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(0);
        }

        public void Remove(string key)
        {
            MemoryCacheDict.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            Remove(key);

            return Task.FromResult(0);
        }

        public void ClearCache()
        {
            MemoryCacheDict = new Dictionary<string, byte[]>();
        }
    }
}
