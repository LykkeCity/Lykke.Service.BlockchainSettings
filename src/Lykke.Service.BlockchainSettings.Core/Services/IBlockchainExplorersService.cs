using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Exceptions;

namespace Lykke.Service.BlockchainSettings.Core.Services
{
    public interface IBlockchainExplorersService
    {
        Task<IEnumerable<BlockchainExplorer>> GetAllAsync();

        Task<IEnumerable<BlockchainExplorer>> GetAsync(string type);

        ///<exception cref="DoesNotExistException">when entity does not exist</exception>
        Task RemoveAsync(string type, string recordId);

        ///<exception cref="AlreadyExistsException">when entity is already created</exception>
        Task CreateAsync(BlockchainExplorer settings);

        ///<exception cref="AlreadyUpdatedException">when entity is already updated</exception>
        Task UpdateAsync(BlockchainExplorer settings);
    }
}
