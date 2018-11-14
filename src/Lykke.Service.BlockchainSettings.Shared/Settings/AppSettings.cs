using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainSettings.Shared.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public BlockchainSettingsSettings BlockchainSettingsService { get; set; }
    }
}
