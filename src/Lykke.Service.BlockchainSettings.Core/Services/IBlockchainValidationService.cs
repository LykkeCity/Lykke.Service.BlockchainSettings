using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Core.Services
{
    public interface IBlockchainValidationService
    {
        Task<bool> ValidateHotwalletAsync(string apiUrl, string type, string address);

        Task<bool> ValidateServiceUrlAsync(string serviceUrl);
    }
}
