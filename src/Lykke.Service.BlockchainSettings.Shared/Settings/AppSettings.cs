using JetBrains.Annotations;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Lykke.Service.BlockchainSettings.Shared.Settings.SlackNotifications;

namespace Lykke.Service.BlockchainSettings.Shared.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public BlockchainSettingsSettings BlockchainSettingsService { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
