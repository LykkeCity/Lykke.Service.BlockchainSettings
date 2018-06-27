using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BlockchainSettings.AzureRepositories.Repositories;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainSettings.Modules
{
    public class DataLayerModule : Module
    {
        private readonly IReloadingManager<BlockchainSettingsSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public DataLayerModule(IReloadingManager<BlockchainSettingsSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<IBlockchainSettingsRepository>(
                BlockchainSettingsRepository.CreateRepository(_settings.ConnectionString(x => x.Db.DataConnectionString),
                    _log));

            builder.Populate(_services);
        }
    }
}
