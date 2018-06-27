using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Exceptions;

namespace Lykke.Service.BlockchainSettings.Core.Services
{
    public interface IBlockchainSettingsService
    {
        Task<IEnumerable<BlockchainSetting>> GetAllAsync();

        Task<(IEnumerable<BlockchainSetting>, string continuationToken)> GetAllAsync(int take, string continuationToken = null);

        Task<BlockchainSetting> GetAsync(string type);

        ///<exception cref="DoesNotExistException">when entity does not exist</exception>
        Task RemoveAsync(string type);

        ///<exception cref="AlreadyExistsException">when entity is already created</exception>
        Task CreateAsync(BlockchainSetting settings);

        ///<exception cref="AlreadyUpdatedException">when entity is already updated</exception>
        Task UpdateAsync(BlockchainSetting settings);
    }
}
