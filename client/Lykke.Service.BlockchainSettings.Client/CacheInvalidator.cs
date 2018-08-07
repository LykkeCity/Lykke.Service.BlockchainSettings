using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lykke.Service.BlockchainSettings.Client.Attributes;

namespace Lykke.Service.BlockchainSettings.Client
{
    public class CacheInvalidator
    {
        private readonly object _locker;
        private readonly IBlockchainSettingsClient _client;
        private IEnumerable<(MethodInfo, InvalidateableClientCachingAttribute)> _methodAttributeTuples;

        public CacheInvalidator(IBlockchainSettingsClient client)
        {
            _client = client;

            var methods = typeof(IBlockchainSettingsClient).GetMethods(BindingFlags.Instance | BindingFlags.Public);

            _methodAttributeTuples = methods
                .Select(x => (x, x.GetCustomAttribute<InvalidateableClientCachingAttribute>()))
                .Where(x => x.Item2 != null);
        }

        public void Invalidate()
        {
            foreach (var tuple in _methodAttributeTuples)
            {
                tuple.Item2.Invalidate();
            }
        }
    }
}
