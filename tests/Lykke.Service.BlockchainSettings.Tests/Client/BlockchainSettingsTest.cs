﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Shared.Cache;
using Lykke.Service.BlockchainSettings.Shared.Security;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Lykke.Service.BlockchainSettings.Tests.Client.Settings;
using Lykke.Service.BlockchainSettings.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Refit;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{

    //Naming: MethodName__TestCase__ExpectedResult
    [TestClass]
    public class BlockchainSettingsTest : BlockchainSettingsTestBase
    {
        public const string ReadKey = "read";
        public const string WriteKey = "write";
        public const string ReadWriteKey = "default";

        [TestMethod]
        public void GetAllSettings__Called__Returns()
        {
            var (factory, fixture) = GenerateControllerFactoryWithFixture(typeof(RegistrationWrapper_GetAllSettings__Called__Returns));
            var client = factory.CreateNew("http://localhost", "default",
                new RequestInterceptorHandler(fixture.Client));
            var allSettings = client.GetAllSettingsAsync().Result;

            Assert.IsNotNull(allSettings.Collection);
            Assert.IsTrue(allSettings.Collection.Count()  == 1);
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

                var blockchainSettingsService = new Mock<IBlockchainSettingsService>();
                blockchainSettingsService.Setup(x => x.GetAllAsync())
                    .Returns(Task.FromResult((IEnumerable<BlockchainSetting>)(new List<BlockchainSetting>()
                    {
                        ethClassicSetting
                    })));

                blockchainSettingsService.Setup(x => x.GetAsync(It.Is<string>(y => y == "EthereumClassic")))
                    .Returns(Task.FromResult(ethClassicSetting));

                blockchainSettingsService.Setup(x => x.CreateAsync(It.IsAny<BlockchainSetting>()))
                    .Returns(Task.FromResult(0));

                blockchainSettingsService.Setup(x => x.RemoveAsync(It.IsAny<string>()))
                    .Returns(Task.FromResult(0));

                blockchainSettingsService.Setup(x => x.UpdateAsync(It.IsAny<BlockchainSetting>()))
                    .Returns(Task.FromResult(0));

                var accessTokenService = new Mock<IAccessTokenService>();
                accessTokenService.Setup(x => x.GetTokenAccess(It.Is<string>(y => y == BlockchainSettingsTest.ReadKey)))
                    .Returns(ApiKeyAccessType.Read);
                accessTokenService.Setup(x => x.GetTokenAccess(It.Is<string>(y => y == BlockchainSettingsTest.WriteKey)))
                    .Returns(ApiKeyAccessType.Write);
                accessTokenService.Setup(x => x.GetTokenAccess(It.Is<string>(y => y == BlockchainSettingsTest.ReadWriteKey)))
                    .Returns(ApiKeyAccessType.ReadWrite);
                #endregion

                BlockchainSettingsServiceCached cachedService = new BlockchainSettingsServiceCached(
                    blockchainSettingsService.Object,
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
