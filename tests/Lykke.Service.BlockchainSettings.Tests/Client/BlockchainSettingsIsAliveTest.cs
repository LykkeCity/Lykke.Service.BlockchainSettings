using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;
using Lykke.Service.BlockchainSettings.Core.Services;
using Lykke.Service.BlockchainSettings.Tests.Client.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{

    //Naming: MethodName__TestCase__ExpectedResult
    [TestClass]
    public class BlockchainSettingsIsAliveTest : BlockchainSettingsTestBase
    {
        [TestMethod]
        public async Task IsAlive__Called__ReturnsResult()
        {
            var (factory, fixture) = GenerateControllerFactoryWithFixture(typeof(RegistrationWrapper_IsAlive__Called__ReturnsResult));
            var (client, cacheManager) = factory.CreateNew("http://localhost", "default", true,
                new RequestInterceptorHandler(fixture.Client));
            var isAliveResponse = await client.GetIsAliveAsync();

            Assert.IsNotNull(isAliveResponse);
        }
    }

    #region ClassFor IsAlive__Called__ReturnsResult
    //Here types lie for startup RegisterContainer methods

    namespace Settings
    {
        public class RegistrationWrapper_IsAlive__Called__ReturnsResult : RegistrationWrapper
        {
            public static Func<IServiceCollection, (IContainer, ILog)> RegisterContainer = (services) =>
            {
                var builder = new ContainerBuilder();
                var log = new LogToConsole();

                #region Mock

                var healthServiceMock = new Mock<IHealthService>();
                healthServiceMock.Setup(x => x.GetHealthIssues()).Returns(
                    (IEnumerable<Lykke.Service.BlockchainSettings.Core.Domain.Health.HealthIssue>)
                    (new List<Lykke.Service.BlockchainSettings.Core.Domain.Health.HealthIssue>()));

                #endregion

                builder.RegisterInstance<ILog>(log).SingleInstance();
                builder.RegisterInstance(healthServiceMock.Object).As<IHealthService>().SingleInstance();
                builder.Populate(services);

                return (builder.Build(), log);
            };

            public RegistrationWrapper_IsAlive__Called__ReturnsResult() : base(RegisterContainer)
            {
            }
        }
    }

    #endregion
}
