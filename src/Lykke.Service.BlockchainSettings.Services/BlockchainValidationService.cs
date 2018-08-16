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

        public async Task<bool> ValidateHotwalletAsync(string apiUrl, string type, string address)
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
                _log.WriteInfo(nameof(ValidateHotwalletAsync), $"{apiUrl}, {type}, {address}", "Could not check validity");
            }

            return isAddressValid;
        }

        public async Task<bool> ValidateServiceUrlAsync(string serviceUrl)
        {
            var isUrlValid = false;

            try
            {
                using (var client = new Lykke.Service.BlockchainApi.Client.BlockchainApiClient(_log, serviceUrl, 3))
                {
                    var isAlive = await client.GetIsAliveAsync();
                    isUrlValid = isAlive != null;
                }
            }
            catch (Exception e)
            {
                _log.WriteInfo(nameof(ValidateHotwalletAsync), $"{serviceUrl}", "Could not check api validity");
            }

            return isUrlValid;
        }
    }
}
