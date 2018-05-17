using System.Collections.Generic;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainSettings.Shared.Security
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly Dictionary<string, ApiKeyAccessType> _apiKeys;

        public AccessTokenService(Dictionary<string, ApiKeyAccessType> apiKeys)
        {
            _apiKeys = apiKeys;
        }

        public ApiKeyAccessType GetTokenAccess(string key)
        {
            _apiKeys.TryGetValue(key, out var accessType);

            return accessType;
        }
    }
}
