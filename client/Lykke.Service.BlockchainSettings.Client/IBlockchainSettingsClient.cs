
using System;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Client.Exception;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using Lykke.Service.BlockchainSettings.Models.Requests;
using Lykke.Service.BlockchainSettings.Models.Responses;
using Refit;

namespace Lykke.Service.BlockchainSettings.Client
{
    public interface IBlockchainSettingsClient
    {
        #region IsAliveController

        /// <summary>
        /// Checks service health
        /// </summary>
        [Get("/api/isAlive")]
        Task<IsAliveResponse> GetIsAliveAsync();

        #endregion

        #region BlockchainSettingsController

        /// <summary>
        /// Get all settings
        /// </summary>
        [Get("/api/blockchain-settings/all")]
        Task<BlockchainSettingsCollectionResponse> GetAllSettingsAsync();

        /// <summary>
        /// Get settings by type
        /// </summary>
        [Get("/api/blockchain-settings/{type}")]
        Task<BlockchainSettingsResponse> GetSettingsByTypeAsync(string type);

        /// <summary>
        /// Create settings
        /// </summary>
        /// <exception cref="NotOkException">Throws in the case of 4xx or 5xx http status code</exception>
        [Post("/api/blockchain-settings")]
        Task CreateAsync(BlockchainSettingsCreateRequest createRequest);

        /// <summary>
        /// Update settings
        /// </summary>
        /// <exception cref="NotOkException">Throws in the case of 4xx or 5xx http status code</exception>
        [Put("/api/blockchain-settings")]
        Task UpdateAsync(BlockchainSettingsUpdateRequest updateRequest);

        /// <summary>
        /// Remove settings
        /// </summary>
        /// <exception cref="NotOkException">Throws in the case of 4xx or 5xx http status code</exception>
        [Delete("/api/blockchain-settings/{type}")]
        Task RemoveAsync(string type);

        #endregion

    }
}
