using Common.Log;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainSettings.Core.Services;
using System;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Services
{
    public class BlockchainValidationService : IBlockchainValidationService
    {
        private ILog _log;

        public BlockchainValidationService(ILog log)
        {
            _log = log.CreateComponentScope(nameof(BlockchainValidationService));
        }

        public async Task<bool> ValidateAsync(string apiUrl, string type, string address)
        {
            var isAddressValid = false;

            try
            {
                //TODO: WAIT FOR NEW IBlockchainApiClient on 2.1 framework (httpClientFactory will be there)
                //DIRTY HACK:
                using (var client = new Lykke.Service.BlockchainApi.Client.BlockchainApiClient(_log, apiUrl, 3))
                {
                    isAddressValid = await client.IsAddressValidAsync(address);
                }
            }
            catch (Exception e)
            {
                _log.WriteInfo(nameof(ValidateAsync), $"{apiUrl}, {type}, {address}", "Could not check validity");
            }

            return isAddressValid;
        }
    }
}
