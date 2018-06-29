using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainSettings.Contract.Requests
{
    [DataContract]
    public class BlockchainSettingsCreateRequest
    {
        [Required]
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [Required]
        [Url]
        [DataMember(Name = "apiUrl")]
        public string ApiUrl { get; set; }

        [Required]
        [Url]
        [DataMember(Name = "signServiceUrl")]
        public string SignServiceUrl { get; set; }

        [Required]
        [DataMember(Name = "hotWalletAddress")]
        public string HotWalletAddress { get; set; }
    }
}
