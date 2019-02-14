using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Client.Exception;
using Lykke.Service.BlockchainSettings.Contract.Requests;
using Lykke.Service.BlockchainSettings.Contract.Responses;
using Refit;
using System.Threading.Tasks;
using Lykke.HttpClientGenerator.Caching;

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
        [ClientCachingAttribute(Minutes = 30)]
        Task<BlockchainSettingsCollectionResponse> GetAllSettingsAsync();

        /// <summary>
        /// Get settings by type
        /// </summary>
        [Get("/api/blockchain-settings/{type}")]
        [ClientCaching(Minutes = 30)]
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

        #region BlockchainExplorersController

        /// <summary>
        /// Get all explorers
        /// </summary>
        [Get("/api/blockchain-explorers/all"), ClientCaching(Minutes = 30)]
        Task<BlockchainExplorersCollectionResponse> GetAllExplorersAsync();

        /// <summary>
        /// Get explorers by type
        /// </summary>
        [Get("/api/blockchain-explorers/{type}")]
        [ClientCaching(Minutes = 30)]
        Task<BlockchainExplorersCollectionResponse> GetBlockchainExplorerByTypeAsync(string type);

        /// <summary>
        /// Get explorers by type
        /// </summary>
        [Get("/api/blockchain-explorers/{type}/{recordId}")]
        [ClientCaching(Minutes = 30)]
        Task<BlockchainExplorersCollectionResponse> GetBlockchainExplorerAsync(string type, string recordId);

        /// <summary>
        /// Create explorer
        /// </summary>
        /// <exception cref="NotOkException">Throws in the case of 4xx or 5xx http status code</exception>
        [Post("/api/blockchain-explorers")]
        Task CreateBlockchainExplorerAsync(BlockchainExplorerCreateRequest createRequest);

        /// <summary>
        /// Update explorer
        /// </summary>
        /// <exception cref="NotOkException">Throws in the case of 4xx or 5xx http status code</exception>
        [Put("/api/blockchain-explorers")]
        Task UpdateBlockchainExplorerAsync(BlockchainExplorerUpdateRequest updateRequest);

        /// <summary>
        /// Remove explorer
        /// </summary>
        /// <exception cref="NotOkException">Throws in the case of 4xx or 5xx http status code</exception>
        [Delete("/api/blockchain-explorers/{type}/{recordId}")]
        Task RemoveBlockchainExplorerAsync(string type, string recordId);

        #endregion

    }
}
