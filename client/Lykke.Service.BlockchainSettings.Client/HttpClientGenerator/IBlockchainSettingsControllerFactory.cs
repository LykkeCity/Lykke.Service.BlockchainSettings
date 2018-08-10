using System.Net.Http;
using Lykke.HttpClientGenerator.Caching;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator
{
    public interface IBlockchainSettingsControllerFactory
    {
        (IBlockchainSettingsClient, IClientCacheManager) CreateNew(string url, string apiKey, bool withCaching, params DelegatingHandler[] handlers);

        (IBlockchainSettingsClient, IClientCacheManager) CreateNew(BlockchainSettingsServiceClientSettings settings,
            bool withCaching = true,
            params DelegatingHandler[] handlers);
    }
}
