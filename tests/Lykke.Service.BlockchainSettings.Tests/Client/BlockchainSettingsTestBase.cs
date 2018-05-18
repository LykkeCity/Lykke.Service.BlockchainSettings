using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers;
using Lykke.Service.BlockchainSettings.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Refit;

namespace Lykke.Service.BlockchainSettings.Tests.Client
{
    //Naming: MethodName__TestCase__ExpectedResult
    public class BlockchainSettingsTestBase
    {
        /// <param name="containerRegistrationWrapper">Type which implements IRegistrationWrapper</param>
        protected (BlockchainSettingsControllerFactory, TestFixture) GenerateControllerFactoryWithFixture(Type containerRegistrationWrapper)
        {
            var startupProxyType = TestHelper.GenerateStartupProxyType(containerRegistrationWrapper);
            var fixture = new TestFixture(startupProxyType, typeof(TestStartup).Assembly, typeof(Startup).Assembly);
            var factory = new BlockchainSettingsControllerFactory();

            return (factory, fixture);
        }
    }
}
