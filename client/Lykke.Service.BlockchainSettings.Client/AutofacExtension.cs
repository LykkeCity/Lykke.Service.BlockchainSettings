//using System;
//using Autofac;
//using Common.Log;

//namespace Lykke.Service.BlockchainSettings.Client
//{
//    public static class AutofacExtension
//    {
//        public static void RegisterBlockchainSettingsClient(this ContainerBuilder builder, string serviceUrl, ILog log)
//        {
//            if (builder == null) throw new ArgumentNullException(nameof(builder));
//            if (log == null) throw new ArgumentNullException(nameof(log));
//            if (string.IsNullOrWhiteSpace(serviceUrl))
//                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

//            builder.RegisterType<BlockchainSettingsClient>()
//                .WithParameter("serviceUrl", serviceUrl)
//                .As<IBlockchainSettingsClient>()
//                .SingleInstance();
//        }

//        public static void RegisterBlockchainSettingsClient(this ContainerBuilder builder, BlockchainSettingsServiceClientSettings settings, ILog log)
//        {
//            builder.RegisterBlockchainSettingsClient(settings?.ServiceUrl, log);
//        }
//    }
//}
