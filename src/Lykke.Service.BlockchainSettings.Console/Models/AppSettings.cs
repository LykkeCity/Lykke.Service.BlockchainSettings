﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BlockchainSettings.Console.Models
{
    [UsedImplicitly]
    public class AppSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public BlockchainsIntegrationSettings BlockchainsIntegration { get; set; }
    }

    [UsedImplicitly]
    public class BlockchainsIntegrationSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IReadOnlyList<BlockchainSettings> Blockchains { get; set; }
    }

    [UsedImplicitly]
    public class BlockchainSettings
    {
        [Optional]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public bool IsDisabled { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Type { get; set; }

        [HttpCheck("/api/isalive")]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string ApiUrl { get; set; }

        [HttpCheck("/api/isalive")]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string SignServiceUrl { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string HotWalletAddress { get; set; }
    }
}
