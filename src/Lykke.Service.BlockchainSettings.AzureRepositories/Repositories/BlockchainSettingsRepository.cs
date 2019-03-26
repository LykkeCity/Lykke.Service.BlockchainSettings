using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Service.BlockchainSettings.AzureRepositories.Entities;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Exceptions;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.BlockchainSettings.AzureRepositories.Repositories
{
    public class BlockchainSettingsRepository : IBlockchainSettingsRepository
    {
        private readonly INoSQLTableStorage<BlockchainSettingEntity> _table;

        public BlockchainSettingsRepository(INoSQLTableStorage<BlockchainSettingEntity> table)
        {
            _table = table;
        }

        public static IBlockchainSettingsRepository CreateRepository(IReloadingManager<string> connectionString, ILogFactory logFactory)
        {
            var repo = new BlockchainSettingsRepository(
                AzureTableStorage<BlockchainSettingEntity>.Create(
                    connectionString, "BlockchainSettings", logFactory));

            return repo;
        }

        public async Task<IEnumerable<BlockchainSetting>> GetAllAsync()
        {
            var entities = await _table.GetDataAsync();
            var mapped = entities.Select(entity => entity.ToDomain());

            return mapped;
        }

        [Obsolete]
        public async Task<(IEnumerable<BlockchainSetting>, string continuationToken)> GetAllAsync(int take,
            string continuationToken = null)
        {
            var (entities, newToken) = await _table.GetDataWithContinuationTokenAsync(take, continuationToken);
            var mapped = entities.Select(entity => entity.ToDomain());

            return (mapped, newToken);
        }

        public async Task<BlockchainSetting> GetAsync(string type)
        {
            var entity = await GetBlockchainSettingEntity(type);
            var mapped = entity?.ToDomain();

            return mapped;
        }

        /// <inheritdoc />
        public async Task CreateAsync(BlockchainSetting settings)
        {
            var existing = await GetBlockchainSettingEntity(settings.Type);

            if (existing != null)
                throw new AlreadyExistsException($"Setting with type {settings.Type} is already exists");

            BlockchainSettingEntity entity = BlockchainSettingEntity.FromDomain(settings);

            await _table.InsertAsync(entity);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(BlockchainSetting settings)
        {
            BlockchainSettingEntity entity = BlockchainSettingEntity.FromDomain(settings);

            string partitionKey = BlockchainSettingEntity.GetPartitionKey(settings.Type);
            string rowKey = BlockchainSettingEntity.GetRowKey(settings.Type);
            string errorMessage = null;

            bool isUpdated = await _table.InsertOrModifyAsync(partitionKey, rowKey, () => entity, model =>
            {
                if (model.ETag != entity.ETag)
                {
                    errorMessage = $"Entity with type {model.Type} has eTag == {model.ETag}, eTag in update request is {entity.ETag}";

                    return false;
                }

                model.Type = entity.Type;
                model.ApiUrl = entity.ApiUrl;
                model.HotWalletAddress = entity.HotWalletAddress;
                model.SignServiceUrl = entity.SignServiceUrl;
                model.ETag = entity.ETag;
                model.PartitionKey = entity.PartitionKey;
                model.RowKey = entity.RowKey;
                model.AreCashinsDisabled = entity.AreCashinsDisabled;
                model.IsExclusiveWithdrawalsRequired = entity.IsExclusiveWithdrawalsRequired;
                model.CashoutAggregationAgeThresholdSeconds = entity.CashoutAggregationAgeThresholdSeconds;
                model.CashoutAggregationCountThreshold = entity.CashoutAggregationCountThreshold;

                return true;
            });

            if (!isUpdated)
                throw new AlreadyUpdatedException(errorMessage);
        }

        /// <inheritdoc />>
        public async Task RemoveAsync(string type)
        {
            string partitionKey = BlockchainSettingEntity.GetPartitionKey(type);
            string rowKey = BlockchainSettingEntity.GetRowKey(type);
            BlockchainSettingEntity existing = await GetBlockchainSettingEntity(type);

            if (existing == null)
                throw new DoesNotExistException($"Settings with type {type} does not exist");

            await _table.DeleteIfExistAsync(partitionKey, rowKey);
        }

        private async Task<BlockchainSettingEntity> GetBlockchainSettingEntity(string type)
        {
            BlockchainSettingEntity entity = await _table.GetDataAsync(BlockchainSettingEntity.GetPartitionKey(type),
                BlockchainSettingEntity.GetRowKey(type));

            return entity;
        }
    }
}
