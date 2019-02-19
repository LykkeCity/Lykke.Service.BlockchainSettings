using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainSettings.Contract.Requests
{
    [DataContract]
    public class BlockchainExplorerUpdateRequest : BlockchainExplorerCreateRequest
    {
        [Required]
        [DataMember(Name = "eTag")]
        public string ETag { get; set; }

        [Required]
        [DataMember(Name = "recordId")]
        public string RecordId { get; set; }
    }
}
