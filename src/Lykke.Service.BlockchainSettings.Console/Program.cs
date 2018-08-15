using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lykke.HttpClientGenerator.Caching;
using Lykke.Service.BlockchainSettings.Client.HttpClientGenerator;
using Lykke.Service.BlockchainSettings.Console.Models;
using Lykke.Service.BlockchainSettings.Contract.Requests;
using Microsoft.Extensions.CommandLineUtils;

namespace Lykke.Service.BlockchainSettings.Console
{
    class Program
    {
        private const string PathToJsonFileAbsolute = "pathToJsonFileAbsolute";
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
                { PathToJsonFileAbsolute, application.Argument(PathToJsonFileAbsolute, "Path to blockchain settings json config.") },
                { BlockchainSettingsUrl, application.Argument(BlockchainSettingsUrl, "Url of a blockchain settings service.") },
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
                            arguments[PathToJsonFileAbsolute].Value,
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

        private static async Task CreateSettingsAsync(string pathToJsonFileAbsolute, string blockchainSettingsUrl, string apiKey)
        {
            if (!File.Exists(pathToJsonFileAbsolute))
            {
                System.Console.WriteLine($"File at {pathToJsonFileAbsolute} does not exist");
                return;
            }

            var text = await File.ReadAllTextAsync(pathToJsonFileAbsolute);
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<BlockchainsList>(text);
            var blockchainSettingsClientFactory = new BlockchainSettingsClientFactory();
            var cacheManager = new ClientCacheManager();
            var client  = blockchainSettingsClientFactory.CreateNew(blockchainSettingsUrl, apiKey, true, cacheManager);

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
                System.Console.WriteLine($"No access to {blockchainSettingsUrl}");
                return;
            }

            foreach (var item in list?.Blockchains)
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
                        ApiUrl = item.ApiUrl
                    });

                    continue;
                }

                await client.CreateAsync(new BlockchainSettingsCreateRequest()
                {
                    Type = item.Type,
                    HotWalletAddress = item.HotWalletAddress,
                    SignServiceUrl = item.SignServiceUrl,
                    ApiUrl = item.ApiUrl,
                });

                System.Console.WriteLine($"{item.Type} has been processed");
            }
        }
    }
}
