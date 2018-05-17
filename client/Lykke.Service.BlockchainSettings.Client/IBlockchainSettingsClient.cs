
using System;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using Refit;

namespace Lykke.Service.BlockchainSettings.Client
{
    public interface IBlockchainSettingsClient : IDisposable
    {
        /// <summary>
        /// Checks service health
        /// </summary>
        [Get("/api/IsAlive")]
        Task<IsAliveResponse> GetIsAliveAsync();
    }
}
