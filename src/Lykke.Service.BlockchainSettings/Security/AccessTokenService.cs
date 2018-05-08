using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.BlockchainSettings.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainSettings.Security
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
