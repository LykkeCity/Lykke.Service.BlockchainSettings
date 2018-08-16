using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using System;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{
    //Naming: MethodName__TestCase__ExpectedResult
    public class BlockchainSettingsTestBase
    {
        /// <param name="containerRegistrationWrapper">Type which implements IRegistrationWrapper</param>
        protected (BlockchainSettingsClientFactory, TestFixture) GenerateControllerFactoryWithFixture(Type containerRegistrationWrapper)
        {
            var startupProxyType = TestHelper.GenerateStartupProxyType(containerRegistrationWrapper);
            var fixture = new TestFixture(startupProxyType, typeof(TestStartup).Assembly, typeof(Startup).Assembly);
            var factory = new BlockchainSettingsClientFactory();

            return (factory, fixture);
        }
    }
}
