using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Core.Services
{
    public interface IBlockchainValidationService
    {
        Task<bool> ValidateAsync(string apiUrl, string type, string address);
    }
}
