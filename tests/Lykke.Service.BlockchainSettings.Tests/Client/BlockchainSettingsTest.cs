using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;
using Lykke.Service.BlockchainSettings.Contract.Requests;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Services;
using Lykke.Service.BlockchainSettings.Shared.Cache;
using Lykke.Service.BlockchainSettings.Shared.Security;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Lykke.Service.BlockchainSettings.Tests.Client.Settings;
using Lykke.Service.BlockchainSettings.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.HttpClientGenerator.Caching;
using NUnit.Framework;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{

    //Naming: MethodName__TestCase__ExpectedResult
    [TestFixture]
    public class BlockchainSettingsTest : BlockchainSettingsTestBase
    {
        public const string ReadKey = "read";
        public const string WriteKey = "write";
        public const string ReadWriteKey = "default";

        [TestCase(true)]
        [TestCase(false)]
        public async Task GetAllSettings__Called__Returns(bool cacheEnabled)
        {
            var (factory, fixture) = GenerateControllerFactoryWithFixture(typeof(RegistrationWrapper_GetAllSettings__Called__Returns));
            var cacheManager = new ClientCacheManager();
            var client = factory.CreateNew("http://localhost", "default", cacheEnabled, cacheManager,
                new RequestInterceptorHandler(fixture.Client));
            var allSettings = await client.GetAllSettingsAsync();

            Assert.IsNotNull(allSettings.Collection);
            Assert.IsTrue(allSettings.Collection.Count()  == 1);
        }

        [Test]
        public async Task GetAllSettings__CalledAfterAdd__CouldBeInvalidated()
        {
            var (factory, fixture) = GenerateControllerFactoryWithFixture(typeof(RegistrationWrapper_GetAllSettings__Called__Returns));
            var cacheManager = new ClientCacheManager();
            var client = factory.CreateNew("http://localhost", "default", true, cacheManager,
                new RequestInterceptorHandler(fixture.Client));

            var allSettings = await client.GetAllSettingsAsync();
            await client.CreateAsync(new BlockchainSettingsCreateRequest()
            {
                HotWalletAddress = "ANY_ADDRESS_POSSIBLE",
                Type = "TYPE_1",
                SignServiceUrl = "http://fake.sign.com/",
                ApiUrl = "http://fake.api.com/"
            });
            var allSettings2 = await client.GetAllSettingsAsync();
            var allSettingsCountBeforeInvalidation = allSettings2.Collection.Count();
            await cacheManager.InvalidateCacheAsync();
            var allSettings3 = await client.GetAllSettingsAsync();

            Assert.IsNotNull(allSettings.Collection);
            Assert.IsNotNull(allSettings2.Collection);
            Assert.IsNotNull(allSettings3.Collection);
            Assert.IsTrue(allSettings.Collection.Count() == 1);
            Assert.IsTrue(allSettingsCountBeforeInvalidation == 1);
            Assert.IsTrue(allSettings3.Collection.Count() == 2);
        }
    }

    #region ClassFor IsAlive__Called__ReturnsResult
    //Here types lie for startup RegisterContainer methods

    namespace Settings
    {
        public class RegistrationWrapper_GetAllSettings__Called__Returns : RegistrationWrapper
        {
            public static Func<IServiceCollection, (IContainer, ILog)> RegisterContainer = (services) =>
            {
                var builder = new ContainerBuilder();
                var log = new LogToConsole();

                #region Mock

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
                #endregion

                BlockchainSettingsServiceCached cachedService = new BlockchainSettingsServiceCached(
                    blockchainSettingsService,
                    new DistributedCacheFake()
                );
                builder.RegisterInstance<IAccessTokenService>(accessTokenService.Object);
                builder.RegisterInstance<IBlockchainSettingsServiceCached>(cachedService).SingleInstance();
                builder.RegisterInstance<ILog>(log).SingleInstance();
                builder.Populate(services);

                return (builder.Build(), log);
            };

            public RegistrationWrapper_GetAllSettings__Called__Returns() : base(RegisterContainer)
            {
            }
        }
    }

    #endregion
}
