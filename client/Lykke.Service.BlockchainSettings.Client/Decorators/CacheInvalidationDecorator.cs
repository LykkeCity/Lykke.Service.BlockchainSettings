using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Contract.Requests;
using Lykke.Service.BlockchainSettings.Contract.Responses;

namespace Lykke.Service.BlockchainSettings.Client.Decorators
{
    public class CacheInvalidationDecorator : IBlockchainSettingsClient
    {
        private readonly IBlockchainSettingsClient _blockchainSettingsClient;

        public CacheInvalidationDecorator(IBlockchainSettingsClient blockchainSettingsClient)
        {
            _blockchainSettingsClient = blockchainSettingsClient;
        }

        public Task<IsAliveResponse> GetIsAliveAsync()
        {
            return _blockchainSettingsClient.GetIsAliveAsync();
        }

        public Task<BlockchainSettingsCollectionResponse> GetAllSettingsAsync()
        {
            return _blockchainSettingsClient.GetAllSettingsAsync();
        }

        public Task<BlockchainSettingsResponse> GetSettingsByTypeAsync(string type)
        {
            return _blockchainSettingsClient.GetSettingsByTypeAsync(type);
        }

        public Task CreateAsync(BlockchainSettingsCreateRequest createRequest)
        {
            throw new NotImplementedException();
            //return _blockchainSettingsClient.GetSettingsByTypeAsync(createRequest);
        }

        public Task UpdateAsync(BlockchainSettingsUpdateRequest updateRequest)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string type)
        {
            throw new NotImplementedException();
        }
    }
}
