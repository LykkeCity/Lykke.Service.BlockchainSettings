using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using NUnit.Framework;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{
    public class BlockchainSettingsTestBase
    {
        protected TestFixture Fixture { get; private set; }
        protected BlockchainSettingsClientFactory Factory { get; private set; }
        
        [OneTimeSetUp]
        public void Init()
        {
            Fixture = new TestFixture();
            Factory = new BlockchainSettingsClientFactory();
        }
    }
}
