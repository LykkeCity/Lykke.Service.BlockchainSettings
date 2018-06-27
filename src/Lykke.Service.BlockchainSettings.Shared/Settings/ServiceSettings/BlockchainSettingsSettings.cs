using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BlockchainSettingsSettings
    {
        public DbSettings Db { get; set; }
        public CacheSettings RedisCache { get; set; }
        public IEnumerable<ApiKey> Keys { get; set; }
    }
}
