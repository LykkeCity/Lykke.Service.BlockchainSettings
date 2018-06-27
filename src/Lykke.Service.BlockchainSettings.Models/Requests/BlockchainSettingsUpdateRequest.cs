﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainSettings.Models.Requests
{
    [DataContract]
    public class BlockchainSettingsUpdateRequest : BlockchainSettingsCreateRequest
    {
        [DataMember(Name = "eTag")]
        [Required]
        public string ETag { get; set; }
    }
}
