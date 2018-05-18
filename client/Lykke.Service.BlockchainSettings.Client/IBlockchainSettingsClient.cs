
using System;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
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
        [Get("/api/IsAlive")]
        Task<IsAliveResponse> GetIsAliveAsync();

        #endregion

        #region BlockchainSettingsController

        /// <summary>
        /// Get all settings
        /// </summary>
        [Get("/api/blockchainSettings/all")]
        Task<BlockchainSettingsCollectionResponse> GetAllSettingsAsync();

        /// <summary>
        /// Get settings by type
        /// </summary>
        [Get("/api/blockchainSettings/{type}")]
        Task<BlockchainSettingsCollectionResponse> GetSettingsByTypeAsync(string type);

        /// <summary>
        /// Create settings
        /// </summary>
        [Post("/api/blockchainSettings")]
        Task<BlockchainSettingsCollectionResponse> CreateAsync(BlockchainSettingsCreateRequest createRequest);

        /// <summary>
        /// Update settings
        /// </summary>
        [Put("/api/blockchainSettings")]
        Task<BlockchainSettingsCollectionResponse> UpdateAsync(BlockchainSettingsUpdateRequest updateRequest);

        /// <summary>
        /// Remove settings
        /// </summary>
        [Delete("/api/blockchainSettings/{type}")]
        Task<BlockchainSettingsCollectionResponse> RemoveAsync(string type);

        #endregion

    }
}
