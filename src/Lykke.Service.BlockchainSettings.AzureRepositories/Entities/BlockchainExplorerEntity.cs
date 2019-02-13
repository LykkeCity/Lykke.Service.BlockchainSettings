using Common;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BlockchainSettings.AzureRepositories.Entities
{
    /*
        "Type": "LiteCoin",
        "ApiUrl": "http://litecoin-api.lykke-service.svc.cluster.local",
        "SignServiceUrl": "${LiteCoinSignServiceUrl}",
        "HotWalletAddress": "mvqYvHSMbEEbQZ4GMnxfc7p5BurnJ95bWd",
     */

    public class BlockchainExplorerEntity : TableEntity
    {
        public string BlockchainType { get; set; }

        public string RecordId { get; set; }

        public string ExplorerUrlTemplate { get; set; }

        public BlockchainExplorer ToDomain()
        {
            return new BlockchainExplorer()
            {
                BlockchainType = this.BlockchainType,
                ExplorerUrlTemplate = this.ExplorerUrlTemplate,
                RecordId = this.RecordId,
                ETag = this.ETag
            };
        }

        public static string GetPartitionKey(string type)
        {
            return type;
        }

        public static string GetRowKey(string recordId)
        {
            return recordId;
        }

        public static BlockchainExplorerEntity FromDomain(BlockchainExplorer explorer)
        {
            return new BlockchainExplorerEntity()
            {
                RecordId = explorer.RecordId,
                BlockchainType = explorer.BlockchainType,
                ExplorerUrlTemplate = explorer.ExplorerUrlTemplate,
                ETag = explorer.ETag,
                PartitionKey = GetPartitionKey(explorer.BlockchainType),
                RowKey = GetRowKey(explorer.RecordId)
            };
        }
    }
}
