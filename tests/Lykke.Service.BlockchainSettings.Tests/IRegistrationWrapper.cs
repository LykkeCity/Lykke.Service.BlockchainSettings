using Autofac;
using Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainSettings.Tests
{
    public interface IRegistrationWrapper
    {
        (IContainer, ILog) RegisterContainer(IServiceCollection services);
    }
}
