using Autofac;
using Lykke.Service.BlockchainSettings.Shared.Security;
using Lykke.SettingsReader;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Service.BlockchainSettings.Shared.Settings;

namespace Lykke.Service.BlockchainSettings.Modules
{
    [UsedImplicitly]
    public class SecurityModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public SecurityModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var dict = _settings.CurrentValue.BlockchainSettingsService.Keys.ToDictionary(x => x.Key, y=> y.AccessType);

            builder.RegisterInstance(new AccessTokenService(dict))
                .As<IAccessTokenService>();
        }
    }
}
