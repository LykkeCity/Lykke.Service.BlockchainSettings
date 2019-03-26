﻿using System;
using Common;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BlockchainSettings.AzureRepositories.Entities
{
    /*
        "Type": "LiteCoin",
        "ApiUrl": "http://litecoin-api.lykke-service.svc.cluster.local",
        "SignServiceUrl": "${LiteCoinSignServiceUrl}",
        "HotWalletAddress": "mvqYvHSMbEEbQZ4GMnxfc7p5BurnJ95bWd",
     */

    public class BlockchainSettingEntity : TableEntity
    {
        public string Type { get; set; }

        public string ApiUrl { get; set; }

        public string SignServiceUrl { get; set; }

        public string HotWalletAddress { get; set; }

        public bool AreCashinsDisabled { get; set; }

        public bool IsExclusiveWithdrawalsRequired { get; set; }

        public int? CashoutAggregationCountThreshold { get; set; }

        public double? CashoutAggregationAgeThresholdSeconds { get; set; }

        public BlockchainSetting ToDomain()
        {
            var cashoutAggregation = this.CashoutAggregationCountThreshold.HasValue &&
                                     this.CashoutAggregationAgeThresholdSeconds.HasValue

                ? new CashoutAggregationSetting()
                {
                    AgeThreshold = TimeSpan.FromSeconds(this.CashoutAggregationAgeThresholdSeconds.Value),
                    CountThreshold = this.CashoutAggregationCountThreshold.Value,
                }
                : null;

            return new BlockchainSetting()
            {
                ApiUrl = this.ApiUrl,
                HotWalletAddress = this.HotWalletAddress,
                SignServiceUrl = this.SignServiceUrl,
                Type = this.Type,
                ETag = this.ETag,
                AreCashinsDisabled = this.AreCashinsDisabled,
                CashoutAggregation = cashoutAggregation,
                IsExclusiveWithdrawalsRequired = this.IsExclusiveWithdrawalsRequired
            };
        }

        public static string GetPartitionKey(string type)
        {
            return type.CalculateHexHash32(3);
        }

        public static string GetRowKey(string type)
        {
            return type;
        }

        public static BlockchainSettingEntity FromDomain(BlockchainSetting settings)
        {
            return new BlockchainSettingEntity()
            {
                ApiUrl = settings.ApiUrl,
                HotWalletAddress = settings.HotWalletAddress,
                SignServiceUrl = settings.SignServiceUrl,
                Type = settings.Type,
                ETag = settings.ETag,
                PartitionKey = GetPartitionKey(settings.Type),
                RowKey = GetRowKey(settings.Type),
                AreCashinsDisabled = settings.AreCashinsDisabled,
                IsExclusiveWithdrawalsRequired = settings.IsExclusiveWithdrawalsRequired,
                CashoutAggregationAgeThresholdSeconds = settings.CashoutAggregation?.AgeThreshold.TotalSeconds,
                CashoutAggregationCountThreshold = settings.CashoutAggregation?.CountThreshold,
            };
        }
    }
}
