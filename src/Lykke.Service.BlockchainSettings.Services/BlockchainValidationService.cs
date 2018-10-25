using Common.Log;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainSettings.Core.Services;
using System;
using System.Threading.Tasks;
using Lykke.Common.Log;

namespace Lykke.Service.BlockchainSettings.Services
{
    public class BlockchainValidationService : IBlockchainValidationService
    {
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        public BlockchainValidationService(ILogFactory logFactory)
        {
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }

        public async Task<bool> ValidateHotwalletAsync(string apiUrl, string type, string address)
        {
            var isAddressValid = false;

            try
            {
                //TODO: WAIT FOR NEW IBlockchainApiClient on 2.1 framework (httpClientFactory will be there)
                //DIRTY HACK:
                using (var client = new BlockchainApiClient(_logFactory, apiUrl, 3))
                {
                    isAddressValid = await client.IsAddressValidAsync(address);
                }
            }
            catch (Exception e)
            {
                _log.Info(nameof(ValidateHotwalletAsync), $"{apiUrl}, {type}, {address}", "Could not check validity");
            }

            return isAddressValid;
        }

        public async Task<bool> ValidateServiceUrlAsync(string serviceUrl)
        {
            var isUrlValid = false;

            try
            {
                using (var client = new BlockchainApiClient(_logFactory, serviceUrl, 3))
                {
                    var isAlive = await client.GetIsAliveAsync();
                    isUrlValid = isAlive != null;
                }
            }
            catch (Exception e)
            {
                _log.Info(nameof(ValidateHotwalletAsync), "Could not check api validity", $"{serviceUrl}");
            }

            return isUrlValid;
        }
    }
}
