using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Core.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message) : base(message)
        {
        }
    }
}
