using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Models.Responses
{
    [DataContract]
    public class BlockchainSettingsCreateRequest
    {
        [DataMember(Name = "eTag")]
        public virtual string ETag { get; set; }

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
