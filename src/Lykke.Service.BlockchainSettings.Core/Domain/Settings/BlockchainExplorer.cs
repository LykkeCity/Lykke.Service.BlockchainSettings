namespace Lykke.Service.BlockchainSettings.Core.Domain.Settings
{
    public class BlockchainExplorer
    {
        public string BlockchainType { get; set; }

        public string RecordId { get; set; }

        public string Name { get; set; }

        public string ETag { get; set; }

        public string ExplorerUrlTemplate { get; set; }
    }
}
