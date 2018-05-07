using System;
using Common.Log;

namespace Lykke.Service.BlockchainSettings.Client
{
    public class BlockchainSettingsClient : IBlockchainSettingsClient, IDisposable
    {
        private readonly ILog _log;

        public BlockchainSettingsClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
