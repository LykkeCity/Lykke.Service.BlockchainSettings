using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.BlockchainSettings.Settings.ServiceSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [Flags()]
    public enum ApiKeyAccessType
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = 3
    }

    public class ApiKey
    {
        public string Key { get; set; }
        public ApiKeyAccessType AccessType { get; set; }
    }

    public class ApiKeys
    {
        public IEnumerable<ApiKey> Keys { get; set; }
    }
}
