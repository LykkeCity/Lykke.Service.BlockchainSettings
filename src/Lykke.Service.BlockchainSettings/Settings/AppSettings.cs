using JetBrains.Annotations;
using Lykke.Service.BlockchainSettings.Settings.ServiceSettings;
using Lykke.Service.BlockchainSettings.Settings.SlackNotifications;

namespace Lykke.Service.BlockchainSettings.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public BlockchainSettingsSettings BlockchainSettingsService { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }

        public ApiKeys ApiKeys { get; set; }

    }
}
