using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;
using System.Threading.Tasks;
using Lykke.HttpClientGenerator.Caching;
using NUnit.Framework;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{
    //Naming: MethodName__TestCase__ExpectedResult
    [TestFixture]
    public class BlockchainSettingsIsAliveTest : BlockchainSettingsTestBase
    {
        //With enabled and disabled cache
        [TestCase(true)]
        [TestCase(false)]
        public async Task IsAlive__Called__ReturnsResult(bool cacheEnabled)
        {
            var cacheManager = new ClientCacheManager();
            var client = Factory.CreateNew(Fixture.ClientUrl, "default", cacheEnabled, cacheManager,
                false,
                new RequestInterceptorHandler(Fixture.Client));
            var isAliveResponse = await client.GetIsAliveAsync();

            Assert.IsNotNull(isAliveResponse);
        }
    }
}
