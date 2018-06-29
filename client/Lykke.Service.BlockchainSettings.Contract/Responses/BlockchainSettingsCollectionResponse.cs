using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainSettings.Contract.Responses
{
    [DataContract]
    public class BlockchainSettingsCollectionResponse
    {
        [DataMember(Name = "collection")]
        public IEnumerable<BlockchainSettingsResponse> Collection { get; set; }
    }
}
