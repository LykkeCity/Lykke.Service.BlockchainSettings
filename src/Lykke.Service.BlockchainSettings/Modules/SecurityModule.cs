using Autofac;
using Common.Log;
using Lykke.Service.BlockchainSettings.Shared.Security;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Lykke.SettingsReader;
using System.Linq;

namespace Lykke.Service.BlockchainSettings.Modules
{
    public class SecurityModule : Module
    {
        private readonly IReloadingManager<ApiKeys> _settings;
        private readonly ILog _log;

        public SecurityModule(IReloadingManager<ApiKeys> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var dict = _settings.CurrentValue.Keys.ToDictionary(x => x.Key, y=> y.AccessType);

            builder.RegisterInstance(new AccessTokenService(dict))
                .As<IAccessTokenService>();
        }
    }
}
