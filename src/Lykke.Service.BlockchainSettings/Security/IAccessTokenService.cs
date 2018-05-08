using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.BlockchainSettings.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainSettings.Security
{
    public interface IAccessTokenService
    {
        ApiKeyAccessType GetTokenAccess(string key);
    }
}
