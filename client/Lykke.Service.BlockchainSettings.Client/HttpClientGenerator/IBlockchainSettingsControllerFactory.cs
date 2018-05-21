using System.Net.Http;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator
{
    public interface IBlockchainSettingsControllerFactory
    {
        IBlockchainSettingsClient CreateNew(string url, string apiKey, bool withCaching, params DelegatingHandler[] handlers);
    }
}
