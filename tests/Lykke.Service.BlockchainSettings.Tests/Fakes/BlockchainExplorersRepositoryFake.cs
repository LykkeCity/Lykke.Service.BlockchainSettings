using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSettings.Core.Exceptions;

namespace Lykke.Service.BlockchainSettings.Tests.Fakes
{
    public class BlockchainExplorersRepositoryFake : IBlockchainExplorersRepository
    {
        public BlockchainExplorersRepositoryFake(IEnumerable<BlockchainExplorer> defindedSettings)
        {
            Explorers = defindedSettings?.ToList() ?? new List<BlockchainExplorer>();
        }

        public List<BlockchainExplorer> Explorers { get; private set; }

        public async Task<(IEnumerable<BlockchainExplorer>, string continuationToken)> GetAllAsync(int take, string continuationToken = null)
        {
            return (Explorers, null);
        }

        public Task<IEnumerable<BlockchainExplorer>> GetAllForBlockchainAsync(string blockchainType)
        {
            return Task.FromResult(Explorers.Where(x => x.BlockchainType == blockchainType));
        }

        public Task<BlockchainExplorer> GetAsync(string blockchainType, string recordId)
        {
            var found = Explorers.FirstOrDefault(x => x.BlockchainType == blockchainType && x.RecordId == recordId);

            return Task.FromResult(found);
        }

        public Task RemoveAsync(string blockchainType, string recordId)
        {
            var explorer = Explorers.FirstOrDefault(x => x.BlockchainType == blockchainType && x.RecordId == recordId);

            if (explorer == null)
                throw  new DoesNotExistException($"");

            Explorers.Remove(explorer);

            return Task.CompletedTask;
        }

        public Task CreateAsync(BlockchainExplorer explorer)
        {
            EnsureETag(explorer);
            Explorers.Add(explorer);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(BlockchainExplorer explorer)
        {
            EnsureETag(explorer);
            var existingExplorer = Explorers.FirstOrDefault(x => x.BlockchainType == explorer.BlockchainType
                                                         && x.RecordId == explorer.RecordId);

            if (explorer == null)
                throw new DoesNotExistException($"");

            Explorers.Remove(existingExplorer);
            Explorers.Add(explorer);

            return Task.CompletedTask;
        }

        private void EnsureETag(BlockchainExplorer explorer)
        {
            if (string.IsNullOrEmpty(explorer.ETag))
                explorer.ETag = DateTime.UtcNow.ToString();
        }
    }
}
