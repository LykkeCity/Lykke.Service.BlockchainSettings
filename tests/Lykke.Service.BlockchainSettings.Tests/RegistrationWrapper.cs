using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainSettings.Tests
{
    public class RegistrationWrapper : IRegistrationWrapper
    {
        private readonly Func<IServiceCollection, (IContainer, ILog)> _registerDelegate;

        public RegistrationWrapper(Func<IServiceCollection, (IContainer, ILog)> registerDelegate)
        {
            _registerDelegate = registerDelegate;
        }

        public virtual (IContainer, ILog) RegisterContainer(IServiceCollection services)
        {
            return _registerDelegate(services);
        }
    }
}
