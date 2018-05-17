using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;
using Lykke.Service.BlockchainSettings.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{
    //Naming: MethodName__TestCase__ExpectedResult
    [TestClass]
    public class BlockchainSettingsTest
    {
        private TestFixture _fixture;
        private BlockchainSettingsControllerFactory _factory;
        public class RegistrationWrapperInit : RegistrationWrapper
        {
            public static Func<IServiceCollection, (IContainer, ILog)> RegisterContainer = (services) =>
            {
                var builder = new ContainerBuilder();
                var log = new LogToConsole();

                #region Mock

                var healthServiceMock = new Mock<IHealthService>();
                healthServiceMock.Setup(x => x.GetHealthIssues()).Returns((IEnumerable<Lykke.Service.BlockchainSettings.Core.Domain.Health.HealthIssue>)
                    (new List<Lykke.Service.BlockchainSettings.Core.Domain.Health.HealthIssue>()));
                #endregion

                builder.RegisterInstance<ILog>(log).SingleInstance();
                builder.RegisterInstance(healthServiceMock.Object).As<IHealthService>().SingleInstance();
                builder.Populate(services);

                return (builder.Build(), log);
            };

            public RegistrationWrapperInit() : base(RegisterContainer)
            {
            }
        }

        [TestInitialize]
        public void Init()
        {
            var startupProxyType = TestHelper.GenerateStartupProxyType(typeof(RegistrationWrapperInit));
            _fixture = new TestFixture(startupProxyType, typeof(TestStartup).Assembly, typeof(Startup).Assembly);
            _factory = new BlockchainSettingsControllerFactory();
        }

        [TestMethod]
        public void IsAlive__Called__ReturnsResult()
        {
            using (var client = _factory.CreateNew("http://localhost", "default", new RequestInterceptorHandler(_fixture.Client)))
            {
                var isAliveResponse = client.GetIsAliveAsync().Result;

                Assert.IsNotNull(isAliveResponse);
            }
        }
    }
}
