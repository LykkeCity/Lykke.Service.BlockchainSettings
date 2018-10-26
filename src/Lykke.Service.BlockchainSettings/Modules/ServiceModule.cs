using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Services;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.BlockchainSettings.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public ServiceModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<BlockchainSettingsService>()
                .As<IBlockchainSettingsService>()
                .SingleInstance();

            builder.RegisterType<BlockchainValidationService>()
                .As<IBlockchainValidationService>()
                .SingleInstance();
        }
    }
}
