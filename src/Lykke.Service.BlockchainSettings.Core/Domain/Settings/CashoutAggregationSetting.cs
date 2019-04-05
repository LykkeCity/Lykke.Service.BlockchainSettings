using System;

namespace Lykke.Service.BlockchainSettings.Core.Domain.Settings
{
    public class CashoutAggregationSetting
    {
        public TimeSpan AgeThreshold { get; set; }

        public int CountThreshold { get; set; }
    }
}