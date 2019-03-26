using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk.Health;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Services;
using Lykke.Service.BlockchainSettings.Shared.Cache;
using Lykke.Service.BlockchainSettings.Shared.Cache.Interfaces;
using Lykke.Service.BlockchainSettings.Shared.Security;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Lykke.Service.BlockchainSettings.Tests.Client;
using Lykke.Service.BlockchainSettings.Tests.Fakes;
using Lykke.SettingsReader;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Tests
{
    [UsedImplicitly]
    public class MocksModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private static BlockchainExplorersRepositoryFake _blockchainExplorersRepository;
        private static BlockchainSettingRepositoryFake _blockchainSettingsRepository;
        private static DistributedCacheFake _cacheFake;

        public MocksModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        public static void ReInitBlockchainExplorersRepository()
        {
            var blockchainExplorer = new BlockchainExplorer()
            {
                BlockchainType = "EthereumClassic",
                ETag = DateTime.UtcNow.ToString(),
                RecordId = Guid.NewGuid().ToString(),
                ExplorerUrlTemplate = "https://some-blockchain-explorer.bit/{tx-hash}",
                Name = "Ropsten"
            };

            var explorers = new List<BlockchainExplorer>()
            {
                blockchainExplorer
            };

            _blockchainExplorersRepository.Explorers.Clear();
            _blockchainExplorersRepository.Explorers.AddRange(explorers);
        }

        public static void ReInitBlockchainSettingsRepository()
        {
            var ethClassicSetting = new BlockchainSetting()
            {
                Type = "EthereumClassic",
                ETag = DateTime.UtcNow.ToString(),
                HotWalletAddress = "0x5ADBF411FAF2595698D80B7f93D570Dd16d7F4B2",
                SignServiceUrl = "http://ethereum-classic-sign.lykke-service.com",
                ApiUrl = "http://ethereum-classic-api.lykke-service.com",
                AreCashinsDisabled = false,
                CashoutAggregation = null,
                IsExclusiveWithdrawalsRequired = false
            };
            var listSettings = new List<BlockchainSetting>()
            {
                ethClassicSetting
            };

            _blockchainSettingsRepository.Settings.Clear();
            _blockchainSettingsRepository.Settings.AddRange(listSettings);
            _cacheFake.ClearCache();
        }

        protected override void Load(ContainerBuilder builder)
        {
            var healthServiceMock = new Mock<IHealthService>();
            healthServiceMock.Setup(x => x.GetHealthIssues()).Returns(new List<HealthIssue>());

            #region BlockchainSettingsService

            _cacheFake = new DistributedCacheFake();
            _blockchainSettingsRepository = new BlockchainSettingRepositoryFake(null);
            ReInitBlockchainSettingsRepository();
            var blockchainValidationService = new Mock<IBlockchainValidationService>();
            blockchainValidationService.Setup(x =>
                    x.ValidateHotwalletAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            blockchainValidationService.Setup(x =>
                    x.ValidateServiceUrlAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            
            var blockchainSettingsService = new BlockchainSettingsService(_blockchainSettingsRepository, 
                blockchainValidationService.Object);
            var accessTokenService = new Mock<IAccessTokenService>();
            accessTokenService.Setup(x => x.GetTokenAccess(It.Is<string>(y => y == BlockchainSettingsTest.ReadKey)))
                .Returns(ApiKeyAccessType.Read);
            accessTokenService.Setup(x => x.GetTokenAccess(It.Is<string>(y => y == BlockchainSettingsTest.WriteKey)))
                .Returns(ApiKeyAccessType.Write);
            accessTokenService.Setup(x => x.GetTokenAccess(It.Is<string>(y => y == BlockchainSettingsTest.ReadWriteKey)))
                .Returns(ApiKeyAccessType.ReadWrite);

            builder.RegisterInstance(healthServiceMock.Object).As<IHealthService>().SingleInstance();
            BlockchainSettingsServiceCached cachedService = new BlockchainSettingsServiceCached(
                blockchainSettingsService,
                _cacheFake
            );
            
            builder.RegisterInstance(accessTokenService.Object);
            builder.RegisterInstance<IBlockchainSettingsServiceCached>(cachedService).SingleInstance();

            #endregion

            #region BlockchainSettingsService

            _blockchainExplorersRepository = new BlockchainExplorersRepositoryFake(null);
            ReInitBlockchainExplorersRepository();
            var blockchainExplorersService = new BlockchainExplorersService(_blockchainExplorersRepository);


            BlockchainExplorersServiceCached cachedExplorersService = new BlockchainExplorersServiceCached(
                blockchainExplorersService,
                new DistributedCacheFake()
            );

            builder.RegisterInstance<IBlockchainExplorersServiceCached>(cachedExplorersService).SingleInstance();

            #endregion
        }
    }
}
