using Lykke.Service.BlockchainSettings.Core.Domain.Settings;
using Lykke.Service.BlockchainSettings.Core.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Core.Repositories
{
    public interface IBlockchainExplorersRepository
    {
        Task<(IEnumerable<BlockchainExplorer>, string continuationToken)> GetAllAsync(int take, string continuationToken = null);

        Task<IEnumerable<BlockchainExplorer>> GetAllForBlockchainAsync(string blockchainType);

        Task<BlockchainExplorer> GetAsync(string blockchainType, string recordId);

        ///<exception cref="DoesNotExistException">when entity does not exist</exception>
        Task RemoveAsync(string blockchainType, string recordId);

        ///<exception cref="AlreadyExistsException">when entity is already created</exception>
        Task CreateAsync(BlockchainExplorer settings);

        ///<exception cref="AlreadyUpdatedException">when entity is already updated</exception>
        Task UpdateAsync(BlockchainExplorer settings);
    }
}
