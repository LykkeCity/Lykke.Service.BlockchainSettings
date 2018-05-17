using Lykke.Service.BlockchainSettings.Models.Requests;
using Lykke.Service.BlockchainSettings.Models.Responses;
using Refit;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator
{
    public interface IBlockchainSettingsController
    {
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
    }
}
