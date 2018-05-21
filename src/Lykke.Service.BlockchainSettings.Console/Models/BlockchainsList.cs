using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Console.Models
{
    public class BlockchainsList
    {
        public IEnumerable<BlockchainSettingsModel> Blockchains { get; set; }
    }

    public class BlockchainSettingsModel
    {
        public string Type { get; set; }
        public string ApiUrl { get; set; }
        public string SignServiceUrl { get; set; }
        public string SignFacadeUrl { get; set; }
        public string SignFacadeApiKey { get; set; }
        public string HotWalletAddress { get; set; }
    }
}
