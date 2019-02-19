using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainSettings.Contract.Responses
{
    [DataContract]
    public class BlockchainExplorersCollectionResponse
    {
        [DataMember(Name = "collection")]
        public IEnumerable<BlockchainExplorerResponse> Collection { get; set; }
    }
}
