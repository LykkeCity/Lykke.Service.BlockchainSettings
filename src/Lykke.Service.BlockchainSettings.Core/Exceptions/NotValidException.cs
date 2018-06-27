using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainSettings.Core.Exceptions
{
    public class NotValidException : Exception
    {
        public NotValidException(string message) : base(message)
        {
        }
    }
}
