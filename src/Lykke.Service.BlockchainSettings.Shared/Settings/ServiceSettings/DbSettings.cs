using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnectionString { get; set; }

        [AzureTableCheck]
        public string DataConnectionString { get; set; }
    }
}
