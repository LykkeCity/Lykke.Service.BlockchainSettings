using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainSettings.Shared.Security
{
    public interface IAccessTokenService
    {
        ApiKeyAccessType GetTokenAccess(string key);
    }
}
