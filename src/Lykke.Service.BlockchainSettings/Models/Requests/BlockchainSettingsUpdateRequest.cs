using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Models.Responses
{
    [DataContract]
    public class BlockchainSettingsUpdateRequest : BlockchainSettingsCreateRequest
    {
        [DataMember(Name = "eTag")]
        [Required]
        public override string ETag { get; set; }
    }
}
