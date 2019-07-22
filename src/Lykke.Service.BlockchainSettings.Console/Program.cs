using Lykke.HttpClientGenerator.Caching;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using Lykke.Service.BlockchainSettings.Console.Models;
using Lykke.Service.BlockchainSettings.Contract;
using Lykke.Service.BlockchainSettings.Contract.Requests;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Console
{
    class Program
    {
        private const string UrlToBlockchainIntegrationService = "urlToBlockchainIntegrationService";
        private const string BlockchainSettingsUrl = "blockchainSettingsUrl";
        private const string ApiKey = "apiKey";

        private static void Main(string[] args)
        {
            var application = new CommandLineApplication
            {
                Description = "Creates blockchain settings from json File"
            };

            var arguments = new Dictionary<string, CommandArgument>
            {
                { UrlToBlockchainIntegrationService,
                    application.Argument(UrlToBlockchainIntegrationService, "Url to blockchain integration service.") },
                { BlockchainSettingsUrl,
                    application.Argument(BlockchainSettingsUrl, "Url of a blockchain settings service.") },
                { ApiKey, application.Argument(BlockchainSettingsUrl, "Api key of a blockchain settings service.") },
            };

            application.HelpOption("-? | -h | --help");
            application.OnExecute(async () =>
            {
                try
                {
                    if (arguments.Any(x => string.IsNullOrEmpty(x.Value.Value)))
                    {
                        application.ShowHelp();
                    }
                    else
                    {
                        await CreateSettingsAsync
                        (
                            arguments[UrlToBlockchainIntegrationService].Value,
                            arguments[BlockchainSettingsUrl].Value,
                            arguments[ApiKey].Value
                        );
                    }

                    return 0;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine();
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(e);

                    return 1;
                }
            });

            application.Execute(args);
        }

        private static async Task CreateSettingsAsync(string urlToSettingsWithBlockchainIntegrationSection, string blockchainSettingsUrl, string apiKey)
        {
            var uri = new Uri(urlToSettingsWithBlockchainIntegrationSection);
            var appSettings = SettingsReader.SettingsReader.ReadGeneralSettings<AppSettings>(uri);
            var list = appSettings.BlockchainsIntegration.Blockchains.ToList();
            var blockchainSettingsClientFactory = new BlockchainSettingsClientFactory();
            var cacheManager = new ClientCacheManager();
            var client = blockchainSettingsClientFactory.CreateNew(blockchainSettingsUrl, apiKey, true, cacheManager);

            try
            {
                var response = await client.GetIsAliveAsync();
                if (response == null)
                {
                    System.Console.WriteLine($"No access to {blockchainSettingsUrl}");
                    return;
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"No access to {blockchainSettingsUrl}. {e}");
                return;
            }

            foreach (var item in list)
            {
                try
                {
                    System.Console.WriteLine($"Processing {item.Type}");
                    var existing = await client.GetSettingsByTypeAsync(item.Type);

                    if (existing != null)
                    {
                        System.Console.WriteLine($"{item.Type} setting already exists");
                        await client.UpdateAsync(new BlockchainSettingsUpdateRequest()
                        {
                            ETag = existing.ETag,
                            Type = item.Type,
                            HotWalletAddress = item.HotWalletAddress,
                            SignServiceUrl = item.SignServiceUrl,
                            ApiUrl = item.ApiUrl,
                            AreCashinsDisabled = item.AreCashinsDisabled,
                            AreCashoutsDisabled = item.AreCashoutsDisabled,
                            CashoutAggregation = item.CashoutAggregation != null
                                ? new CashoutAggregationSettingDto()
                                {
                                    CountThreshold = item.CashoutAggregation.CountThreshold,
                                    AgeThreshold = item.CashoutAggregation.AgeThreshold,
                                }
                                : null,
                            IsExclusiveWithdrawalsRequired = item.ExclusiveWithdrawalsRequired
                        });

                        continue;
                    }

                    await client.CreateAsync(new BlockchainSettingsCreateRequest()
                    {
                        Type = item.Type,
                        HotWalletAddress = item.HotWalletAddress,
                        SignServiceUrl = item.SignServiceUrl,
                        ApiUrl = item.ApiUrl,
                        AreCashinsDisabled = item.AreCashinsDisabled,
                        AreCashoutsDisabled = item.AreCashoutsDisabled,
                        CashoutAggregation = item.CashoutAggregation != null
                            ? new CashoutAggregationSettingDto()
                            {
                                CountThreshold = item.CashoutAggregation.CountThreshold,
                                AgeThreshold = item.CashoutAggregation.AgeThreshold,
                            }
                            : null,
                        IsExclusiveWithdrawalsRequired = item.ExclusiveWithdrawalsRequired
                    });

                    System.Console.WriteLine($"{item.Type} has been processed");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"Exception has appeared during processing {item.Type}");
                    System.Console.WriteLine(e.Message);
                    System.Console.WriteLine();
                    System.Console.WriteLine($"Skipping {item.Type}");
                }
            }
        }
    }
}
