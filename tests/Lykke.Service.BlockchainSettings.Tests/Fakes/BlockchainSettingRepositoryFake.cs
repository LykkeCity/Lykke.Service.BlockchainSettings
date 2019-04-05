using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Tests.Fakes
{
    public class BlockchainSettingRepositoryFake : IBlockchainSettingsRepository
    {
        public BlockchainSettingRepositoryFake(IEnumerable<BlockchainSetting> defindedSettings)
        {
            Settings = defindedSettings?.ToList() ?? new List<BlockchainSetting>();
        }

        public List<BlockchainSetting> Settings { get; private set; }

        public Task<IEnumerable<BlockchainSetting>> GetAllAsync()
        {
            return Task.FromResult((IEnumerable<BlockchainSetting>)Settings);
        }

        public async Task<(IEnumerable<BlockchainSetting>, string continuationToken)> GetAllAsync(int take, string continuationToken = null)
        {
            return (await GetAllAsync(), null);
        }

        public Task<BlockchainSetting> GetAsync(string type)
        {
            var found = Settings.FirstOrDefault(x => x.Type == type);

            return Task.FromResult(found);
        }

        public Task RemoveAsync(string type)
        {
            var found = Settings.FirstOrDefault(x => x.Type == type);
             
            return Task.FromResult(found);
        }

        public Task CreateAsync(BlockchainSetting settings)
        {
            Settings.Add(settings);

            return Task.FromResult(0);
        }

        public Task UpdateAsync(BlockchainSetting settings)
        {
            throw new NotImplementedException();
        }
    }
}
