using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}