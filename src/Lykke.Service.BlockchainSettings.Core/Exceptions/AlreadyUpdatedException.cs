using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Core.Exceptions
{
    public class AlreadyUpdatedException : Exception
    {
        public AlreadyUpdatedException(string message) : base(message)
        {
        }
    }
}
