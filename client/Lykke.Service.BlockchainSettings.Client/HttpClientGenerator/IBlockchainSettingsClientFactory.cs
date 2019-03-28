using System.Net.Http;
using Lykke.HttpClientGenerator.Caching;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator
{
    public interface IBlockchainSettingsClientFactory
    {
        IBlockchainSettingsClient CreateNew(BlockchainSettingsServiceClientSettings settings,
            bool withCaching = true,
            IClientCacheManager clientCacheManager = null,
            bool disableRetries = false,
            params DelegatingHandler[] handlers);

        IBlockchainSettingsClient CreateNew(string url, string apiKey, bool withCaching = true,
            IClientCacheManager clientCacheManager = null,
            bool disableRetries = false, 
            params DelegatingHandler[] handlers);
    }
}
