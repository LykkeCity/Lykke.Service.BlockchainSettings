using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings
{
    public class CacheSettings
    {
        [Optional]
        public string InstanceName { get; set; }
        public string RedisConfiguration { get; set; }
    }
}
