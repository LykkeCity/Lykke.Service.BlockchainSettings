using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Core.Exceptions
{
    public class DoesNotExistException : Exception
    {
        public DoesNotExistException(string message) : base(message)
        {
        }
    }
}
