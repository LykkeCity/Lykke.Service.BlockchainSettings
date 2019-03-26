using JetBrains.Annotations;

namespace Lykke.Service.BlockchainSettings.Core.Domain.Settings
{
    public class BlockchainSetting
    {
        public string ETag { get; set; }

        public string Type { get; set; }

        public string ApiUrl { get; set; }

        public string SignServiceUrl { get; set; }

        public string HotWalletAddress { get; set; }

        public bool AreCashinsDisabled { get; set; }

        public bool IsExclusiveWithdrawalsRequired { get; set; }

        [CanBeNull]
        public CashoutAggregationSetting CashoutAggregation { get; set; }
    }
}
