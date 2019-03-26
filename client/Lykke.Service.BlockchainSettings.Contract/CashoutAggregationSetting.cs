using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainSettings.Contract
{
    [DataContract]
    public class CashoutAggregationSettingDto
    {
        [Required]
        [DataMember(Name = "ageThreshold")]
        public TimeSpan AgeThreshold { get; set; }

        [Required]
        [DataMember(Name = "countThreshold")]
        public int CountThreshold { get; set; }
    }
}
