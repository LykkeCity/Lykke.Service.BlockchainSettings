using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Tests.Fakes
{
    public class BlockchainExplorersRepositoryFake : IBlockchainExplorersRepository
    {
        public BlockchainExplorersRepositoryFake(IEnumerable<BlockchainExplorer> defindedSettings)
        {
            Explorers = defindedSettings.ToList();
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
            throw new NotImplementedException();
        }

        public Task CreateAsync(BlockchainExplorer settings)
        {
            Explorers.Add(settings);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(BlockchainExplorer settings)
        {
            throw new NotImplementedException();
        }
    }
}
