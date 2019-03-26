using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainSettings.Contract.Responses
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

        [DataMember(Name = "areCashinsDisabled")]
        public bool AreCashinsDisabled { get; set; }

        [DataMember(Name = "isExclusiveWithdrawalsRequired")]
        public bool IsExclusiveWithdrawalsRequired { get; set; }

        [DataMember(Name = "cashoutAggregation")]
        public CashoutAggregationSettingDto CashoutAggregation { get; set; }
    }
}
