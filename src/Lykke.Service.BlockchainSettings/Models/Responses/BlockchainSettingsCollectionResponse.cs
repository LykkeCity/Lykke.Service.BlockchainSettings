using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Models.Responses
{
    [DataContract]
    public class BlockchainSettingsCollectionResponse
    {
        [DataMember(Name = "collection")]
        public IEnumerable<BlockchainSettingsResponse> Collection { get; set; }
    }
}
