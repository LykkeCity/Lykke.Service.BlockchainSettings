using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BlockchainSettings.Shared.Settings
{
    public class MonitoringServiceClientSettings
    {
        [HttpCheck("api/isalive", false)]
        public string MonitoringServiceUrl { get; set; }
    }
}
