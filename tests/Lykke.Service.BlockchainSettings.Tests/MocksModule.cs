using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk.Health;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Services;
using Lykke.Service.BlockchainSettings.Shared.Cache;
using Lykke.Service.BlockchainSettings.Shared.Security;
using Lykke.Service.BlockchainSettings.Shared.Settings;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Lykke.Service.BlockchainSettings.Tests.Client;
using Lykke.Service.BlockchainSettings.Tests.Fakes;
using Lykke.SettingsReader;
using Moq;

namespace Lykke.Service.BlockchainSettings.Tests
{
    [UsedImplicitly]
    public class MocksModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public MocksModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var healthServiceMock = new Mock<IHealthService>();
            healthServiceMock.Setup(x => x.GetHealthIssues()).Returns(new List<HealthIssue>());

            var ethClassicSetting = new BlockchainSetting()
            {
                Type = "EthereumClassic",
                ETag = DateTime.UtcNow.ToString(),
                HotWalletAddress = "0x5ADBF411FAF2595698D80B7f93D570Dd16d7F4B2",
                SignServiceUrl = "http://ethereum-classic-sign.lykke-service.com",
                ApiUrl = "http://ethereum-classic-api.lykke-service.com"
            };
            var listSettings = new List<BlockchainSetting>()
            {
                ethClassicSetting
            };

            var blockchainValidationService = new Mock<IBlockchainValidationService>();
            blockchainValidationService.Setup(x =>
                    x.ValidateHotwalletAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            blockchainValidationService.Setup(x =>
                    x.ValidateServiceUrlAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            var blockchainSettingsRepository = new BlockchainSettingRepositoryFake(listSettings);
            var blockchainSettingsService = new BlockchainSettingsService(blockchainSettingsRepository, 
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
                new DistributedCacheFake()
            );
            
            builder.RegisterInstance(accessTokenService.Object);
            builder.RegisterInstance<IBlockchainSettingsServiceCached>(cachedService).SingleInstance();
        }
    }
}
