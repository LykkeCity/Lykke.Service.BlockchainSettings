using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BlockchainSettings.Console.Models
{
    [UsedImplicitly]
    public class AppSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public BlockchainsIntegrationSettings BlockchainsIntegration { get; set; }
    }

    [UsedImplicitly]
    public class BlockchainsIntegrationSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IReadOnlyList<BlockchainSettings> Blockchains { get; set; }
    }

    [UsedImplicitly]
    public class BlockchainSettings
    {
        [Optional]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public bool IsDisabled { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Type { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string ApiUrl { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string SignServiceUrl { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string HotWalletAddress { get; set; }

        [Optional]
        public bool AreCashinsDisabled { get; set; }

        [Optional]
        public bool AreCashoutsDisabled { get; set; }

        [Optional]
        public bool IsExclusiveWithdrawalsRequired { get; set; }

        [Optional]
        public CashoutAggregationModel CashoutAggregation { get; set; }
    }

    public class CashoutAggregationModel
    {
        public int CountThreshold { get; set; }

        public TimeSpan AgeThreshold { get; set; }
    }
}
