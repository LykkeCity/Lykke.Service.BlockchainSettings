using Lykke.Common.Api.Contract.Responses;
using Refit;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator
{
    public interface IIsAliveController
    {
        /// <summary>
        /// Checks service health
        /// </summary>
        [Get("/api/IsAlive")]
        Task<IsAliveResponse> GetIsAliveAsync();
    }
}
