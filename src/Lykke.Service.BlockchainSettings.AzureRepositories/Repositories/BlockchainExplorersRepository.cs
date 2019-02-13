using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Service.BlockchainSettings.AzureRepositories.Entities;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Exceptions;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using Lykke.SettingsReader;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.AzureRepositories.Repositories
{
    public class BlockchainExplorersRepository : IBlockchainExplorersRepository
    {
        private readonly INoSQLTableStorage<BlockchainExplorerEntity> _table;

        public BlockchainExplorersRepository(INoSQLTableStorage<BlockchainExplorerEntity> table)
        {
            _table = table;
        }

        public static IBlockchainExplorersRepository CreateRepository(IReloadingManager<string> connectionString, ILogFactory logFactory)
        {
            var repo = new BlockchainExplorersRepository(
                AzureTableStorage<BlockchainExplorerEntity>.Create(
                    connectionString, "BlockchainExplorers", logFactory));

            return repo;
        }

        public async Task<(IEnumerable<BlockchainExplorer>, string continuationToken)> GetAllAsync(int take,
            string continuationToken = null)
        {
            var (entities, newToken) = await _table.GetDataWithContinuationTokenAsync(take, continuationToken);
            var mapped = entities.Select(entity => entity.ToDomain());

            return (mapped, newToken);
        }

        public async Task<IEnumerable<BlockchainExplorer>> GetAllForBlockchainAsync(string blockchainType)
        {
            var allExplorers = await _table.GetDataAsync(BlockchainExplorerEntity.GetPartitionKey(blockchainType));
            var mapped = allExplorers.Select(entity => entity.ToDomain());

            return mapped;
        }

        public async Task<BlockchainExplorer> GetAsync(string type, string recordId)
        {
            var entity = await GetBlockchainExplorerEntity(type, recordId);
            var mapped = entity?.ToDomain();

            return mapped;
        }

        /// <inheritdoc />
        public async Task CreateAsync(BlockchainExplorer explorer)
        {
            var existing = await GetBlockchainExplorerEntity(explorer.BlockchainType, explorer.RecordId);

            if (existing != null)
                throw new AlreadyExistsException($"Setting with type {explorer.BlockchainType} is already exists");

            BlockchainExplorerEntity entity = BlockchainExplorerEntity.FromDomain(explorer);

            await _table.InsertAsync(entity);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(BlockchainExplorer explorer)
        {
            BlockchainExplorerEntity entity = BlockchainExplorerEntity.FromDomain(explorer);

            string partitionKey = BlockchainExplorerEntity.GetPartitionKey(explorer.BlockchainType);
            string rowKey = BlockchainExplorerEntity.GetRowKey(explorer.RecordId);
            string errorMessage = null;

            bool isUpdated = await _table.InsertOrModifyAsync(partitionKey, rowKey, () => entity, model =>
            {
                if (model.ETag != entity.ETag)
                {
                    errorMessage = $"Entity with type {model.BlockchainType} has eTag == {model.ETag}, eTag in update request is {entity.ETag}";

                    return false;
                }

                model.BlockchainType = entity.BlockchainType;
                model.ExplorerUrlTemplate = entity.ExplorerUrlTemplate;
                model.RecordId = entity.RecordId;
                model.ETag = entity.ETag;
                model.PartitionKey = entity.PartitionKey;
                model.RowKey = entity.RowKey;

                return true;
            });

            if (!isUpdated)
                throw new AlreadyUpdatedException(errorMessage);
        }

        /// <inheritdoc />>
        public async Task RemoveAsync(string type, string recordId)
        {
            string partitionKey = BlockchainExplorerEntity.GetPartitionKey(type);
            string rowKey = BlockchainExplorerEntity.GetRowKey(recordId);
            var existing = await GetBlockchainExplorerEntity(type, recordId);

            if (existing == null)
                throw new DoesNotExistException($"Settings with type {type} does not exist");

            await _table.DeleteIfExistAsync(partitionKey, rowKey);
        }

        private async Task<BlockchainExplorerEntity> GetBlockchainExplorerEntity(string type, string recordId)
        {
            BlockchainExplorerEntity entity = await _table.GetDataAsync(BlockchainExplorerEntity.GetPartitionKey(type),
                BlockchainExplorerEntity.GetRowKey(recordId));

            return entity;
        }
    }
}
