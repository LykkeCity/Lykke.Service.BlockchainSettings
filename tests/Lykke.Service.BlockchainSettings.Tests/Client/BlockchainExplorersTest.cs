using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;
using Lykke.Service.BlockchainSettings.Contract.Requests;
using System.Linq;
using System.Threading.Tasks;
using Lykke.HttpClientGenerator.Caching;
using NUnit.Framework;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{
    //Naming: MethodName__TestCase__ExpectedResult
    [TestFixture]
    public class BlockchainExplorersTest : BlockchainSettingsTestBase
    {
        public const string ReadKey = "read";
        public const string WriteKey = "write";
        public const string ReadWriteKey = "default";

        [TestCase(true)]
        [TestCase(false)]
        public async Task GetAllSettings__Called__Returns(bool cacheEnabled)
        {
            var cacheManager = new ClientCacheManager();
            var client = Factory.CreateNew(Fixture.ClientUrl, "default", cacheEnabled, cacheManager,
                new RequestInterceptorHandler(Fixture.Client));
            var allSettings = await client.GetAllExplorersAsync();

            Assert.IsNotNull(allSettings.Collection);
            Assert.IsTrue(allSettings.Collection.Count()  == 1);
        }

        [Test]
        public async Task GetAllSettings__CalledAfterAdd__CouldBeInvalidated()
        {
            var cacheManager = new ClientCacheManager();
            var client = Factory.CreateNew(Fixture.ClientUrl, "default", true, cacheManager,
                new RequestInterceptorHandler(Fixture.Client));

            var allSettings = await client.GetAllExplorersAsync();
            await client.CreateBlockchainExplorerAsync(new BlockchainExplorerCreateRequest
            {
                BlockchainType = "EthereumClassic",
                ExplorerUrlTemplate = "https://ropsten.etherscan.io/tx/{tx-hash}"
            });
            var allSettings2 = await client.GetAllExplorersAsync();
            var allSettingsCountBeforeInvalidation = allSettings2.Collection.Count();
            await cacheManager.InvalidateCacheAsync();
            var allSettings3 = await client.GetAllExplorersAsync();

            Assert.IsNotNull(allSettings.Collection);
            Assert.IsNotNull(allSettings2.Collection);
            Assert.IsNotNull(allSettings3.Collection);
            Assert.IsTrue(allSettings.Collection.Count() == 1);
            Assert.IsTrue(allSettingsCountBeforeInvalidation == 1);
            Assert.IsTrue(allSettings3.Collection.Count() == 2);
        }
    }
}
