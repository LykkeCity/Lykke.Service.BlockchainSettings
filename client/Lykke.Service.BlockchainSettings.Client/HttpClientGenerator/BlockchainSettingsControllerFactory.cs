using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Lykke.HttpClientGenerator;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator
{
    public class BlockchainSettingsControllerFactory : IBlockchainSettingsControllerFactory
    {
        public IBlockchainSettingsClient CreateNew(string url, string apiKey, params DelegatingHandler[] handlers)
        {
            var builder = new HttpClientGeneratorBuilder(url)
                .WithAdditionalDelegatingHandler(new ApiKeyHeaderHandler(apiKey))
                .WithAdditionalDelegatingHandler(new ResponseHandler());

            foreach (var handler in handlers)
            {
                builder.WithAdditionalDelegatingHandler(handler);
            }

            var httpClientGenerator = builder.Create();

            return httpClientGenerator.Generate<IBlockchainSettingsClient>();
        }
    }
}
