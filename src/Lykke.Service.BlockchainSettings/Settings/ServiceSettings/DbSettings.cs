using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BlockchainSettings.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
