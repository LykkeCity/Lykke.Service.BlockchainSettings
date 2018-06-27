using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.BlockchainSettings.Tests.Fakes
{
    public class BlockchainSettingsServiceFake : IBlockchainSettingsService
    {
        public BlockchainSettingsServiceFake(IEnumerable<BlockchainSetting> defindedSettings)
        {
            Settings = defindedSettings.ToList();
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
            throw new NotImplementedException();
        }

        public Task UpdateAsync(BlockchainSetting settings)
        {
            throw new NotImplementedException();
        }
    }
}
