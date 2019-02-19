using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainSettings.Contract.Requests
{
    [DataContract]
    public class BlockchainExplorerCreateRequest
    {
        [Required]
        [DataMember(Name = "blockchainType")]
        public string BlockchainType { get; set; }

        [Required]
        [DataMember(Name = "explorerUrlTemplate")]
        public string ExplorerUrlTemplate { get; set; }
    }
}
