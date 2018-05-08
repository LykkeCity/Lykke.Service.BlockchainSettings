using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Models.Responses
{
    [DataContract]
    public class BlockchainSettingsResponse
    {
        [DataMember(Name = "eTag")]
        public string ETag { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "apiUrl")]
        public string ApiUrl { get; set; }

        [DataMember(Name = "signServiceUrl")]
        public string SignServiceUrl { get; set; }

        [DataMember(Name = "hotWalletAddress")]
        public string HotWalletAddress { get; set; }
    }
}
