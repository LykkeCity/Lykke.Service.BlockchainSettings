using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
