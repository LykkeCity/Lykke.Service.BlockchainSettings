using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Client.Exception
{
    public class NotOkException : System.Exception
    {
        public int HttpCode { get; set; }

        public NotOkException(int httpCode, string message) : base(message)
        {
            HttpCode = httpCode;
        }
    }
}
