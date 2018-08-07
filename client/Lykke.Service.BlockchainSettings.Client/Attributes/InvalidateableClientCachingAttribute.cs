using Lykke.HttpClientGenerator.Caching;
using System;

namespace Lykke.Service.BlockchainSettings.Client.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class InvalidateableClientCachingAttribute : ClientCachingAttribute
    {
        public InvalidateableClientCachingAttribute() : base()
        {
        }

        public InvalidateableClientCachingAttribute(string cachingTimeString) : base(cachingTimeString)
        {
        }

        public void Invalidate()
        {
            base.Hours = 0;
            base.Minutes = 0;
            base.Seconds = 0;
        }
    }
}
