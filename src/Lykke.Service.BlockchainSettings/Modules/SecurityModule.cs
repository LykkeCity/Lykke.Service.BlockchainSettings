using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Security;
using Lykke.Service.BlockchainSettings.Settings.ServiceSettings;
using Lykke.Service.BlockchainSettings.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

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
