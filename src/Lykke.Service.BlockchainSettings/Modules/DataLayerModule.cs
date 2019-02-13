using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.BlockchainSettings.AzureRepositories.Repositories;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.BlockchainSettings.Modules
{
    [UsedImplicitly]
    public class DataLayerModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public DataLayerModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                BlockchainSettingsRepository.CreateRepository(_settings.ConnectionString(x => x.BlockchainSettingsService.Db.DataConnectionString),
                    c.Resolve<ILogFactory>()))
                .As<IBlockchainSettingsRepository>()
                .SingleInstance();

            builder.Register(c =>
                    BlockchainExplorersRepository.CreateRepository(_settings.ConnectionString(x => x.BlockchainSettingsService.Db.DataConnectionString),
                        c.Resolve<ILogFactory>()))
                .As<IBlockchainExplorersRepository>()
                .SingleInstance();
        }
    }
}
