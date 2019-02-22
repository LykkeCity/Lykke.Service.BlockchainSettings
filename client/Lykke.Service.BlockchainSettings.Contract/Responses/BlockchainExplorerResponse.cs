using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Contract.Responses
{
    [DataContract]
    public class BlockchainExplorerResponse
    {
        [DataMember(Name = "blockchainType")]
        public string BlockchainType { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "recordId")]
        public string RecordId { get; set; }

        [DataMember(Name = "eTag")]
        public string ETag { get; set; }

        [DataMember(Name = "explorerUrlTemplate")]
        public string ExplorerUrlTemplate { get; set; }
    }
}
