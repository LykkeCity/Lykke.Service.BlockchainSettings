using Lykke.HttpClientGenerator;
using Lykke.HttpClientGenerator.Caching;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;
using System.Net.Http;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class BlockchainSettingsClientFactory : IBlockchainSettingsClientFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="withCaching"></param>
        /// <param name="clientCacheManager"></param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public IBlockchainSettingsClient CreateNew(BlockchainSettingsServiceClientSettings settings,
            bool withCaching = true,
            IClientCacheManager clientCacheManager = null,
            params DelegatingHandler[] handlers)
        {
            return CreateNew(settings?.ServiceUrl, settings?.ApiKey, withCaching, clientCacheManager, handlers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="apiKey"></param>
        /// <param name="withCaching"></param>
        /// <param name="clientCacheManager"></param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public IBlockchainSettingsClient CreateNew(string url, string apiKey, bool withCaching = true,
            IClientCacheManager clientCacheManager = null, params DelegatingHandler[] handlers)
        {
            var builder = new HttpClientGeneratorBuilder(url)
                .WithAdditionalDelegatingHandler(new ApiKeyHeaderHandler(apiKey))
                .WithAdditionalDelegatingHandler(new ResponseHandler());

            if (withCaching)
            {
                //explicit strategy declaration
                builder.WithCachingStrategy(new AttributeBasedCachingStrategy());
            }
            else
            {
                //By default it is AttributeBasedCachingStrategy, so if no caching turn it off
                builder.WithoutCaching();
            }

            foreach (var handler in handlers)
            {
                builder.WithAdditionalDelegatingHandler(handler);
            }

            clientCacheManager = clientCacheManager ?? new ClientCacheManager();
            var httpClientGenerator = builder.Create(clientCacheManager);

            return httpClientGenerator.Generate<IBlockchainSettingsClient>();
        }
    }
}
