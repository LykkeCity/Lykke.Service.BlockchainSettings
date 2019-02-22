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

        [SetUp]
        public void SetupBeforeEachTest()
        {
            MocksModule.ReInitBlockchainRepository();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task GetAllExplorersAsync__Called__Returns(bool cacheEnabled)
        {
            var cacheManager = new ClientCacheManager();
            var client = Factory.CreateNew(Fixture.ClientUrl, "default", cacheEnabled, cacheManager,
                new RequestInterceptorHandler(Fixture.Client));
            var allSettings = await client.GetAllExplorersAsync();

            Assert.IsNotNull(allSettings.Collection);
            Assert.IsTrue(allSettings.Collection.Count() == 1);
        }

        [Test]
        public async Task GetAllExplorersAsync__CalledAfterAdd__CouldBeInvalidated()
        {
            var cacheManager = new ClientCacheManager();
            var client = Factory.CreateNew(Fixture.ClientUrl, "default", true, cacheManager,
                new RequestInterceptorHandler(Fixture.Client));

            var allSettings = await client.GetAllExplorersAsync();
            await client.CreateBlockchainExplorerAsync(new BlockchainExplorerCreateRequest
            {
                Name = "Ropsten1",
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

        [Test]
        public async Task CheckBasicFlow()
        {
            var client = Factory.CreateNew(Fixture.ClientUrl, "default", false, null,
                new RequestInterceptorHandler(Fixture.Client));
            var ropstenTemplate = "https://ropsten.etherscan.io/tx/{tx-hash}";
            var mainnetTemplate = "https://etherscan.io/tx/{tx-hash}";
            var blockchainType = "EthereumClassic";

            await client.CreateBlockchainExplorerAsync(new BlockchainExplorerCreateRequest
            {
                Name = "RopstenX",
                BlockchainType = blockchainType,
                ExplorerUrlTemplate = ropstenTemplate
            });

            var allSettings2 = await client.GetAllExplorersAsync();
            var ropstenEthereum = allSettings2
                .Collection
                .FirstOrDefault(x => x.ExplorerUrlTemplate == ropstenTemplate);

            ropstenEthereum.ExplorerUrlTemplate = mainnetTemplate;

            await client.UpdateBlockchainExplorerAsync(new BlockchainExplorerUpdateRequest()
            {
                Name = "Ropsten1",
                BlockchainType = ropstenEthereum.BlockchainType,
                RecordId = ropstenEthereum.RecordId,
                ExplorerUrlTemplate = ropstenEthereum.ExplorerUrlTemplate,
                ETag = ropstenEthereum.ETag
            });

            var allSettings3 = await client.GetBlockchainExplorerByTypeAsync(blockchainType);
            var mainnetEthereum = allSettings3
                .Collection
                .FirstOrDefault(x => x.ExplorerUrlTemplate == mainnetTemplate);

            Assert.IsTrue(mainnetEthereum != null);
            Assert.IsTrue(mainnetEthereum.Name == "Ropsten1");

            var record =
                await client.GetBlockchainExplorerAsync(mainnetEthereum.BlockchainType, mainnetEthereum.RecordId);
            await client.RemoveBlockchainExplorerAsync(mainnetEthereum.BlockchainType, mainnetEthereum.RecordId);
            var allSettings4 = await client.GetBlockchainExplorerByTypeAsync(blockchainType);
            Assert.IsTrue(allSettings4.Collection.Count() == 1);
            Assert.IsNotNull(record);
        }

        [Test]
        public async Task CreateBlockchainExplorerAsync__UrlIsNotValid__ThrowsNotOkException()
        {
            var client = Factory.CreateNew(Fixture.ClientUrl, "default", false, null,
                new RequestInterceptorHandler(Fixture.Client));
            var ropstenTemplate = "http://stellar/m                     ail1/{tx-hash}";
            var blockchainType = "EthereumClassic";

            await client.CreateBlockchainExplorerAsync(new BlockchainExplorerCreateRequest
            {
                BlockchainType = blockchainType,
                ExplorerUrlTemplate = ropstenTemplate,
                Name = "Ropsten1"
            });
        }
    }
}
