using JetBrains.Annotations;

namespace Lykke.Service.BlockchainSettings.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BlockchainSettingsSettings
    {
        public DbSettings Db { get; set; }
        public CacheSettings RedisCache { get; set; }
    }
}
