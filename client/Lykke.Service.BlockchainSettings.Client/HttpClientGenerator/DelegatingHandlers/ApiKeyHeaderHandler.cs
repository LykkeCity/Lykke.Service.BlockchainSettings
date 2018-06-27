﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers
{
    internal class ApiKeyHeaderHandler : DelegatingHandler
    {
        private readonly string _apiKey;

        public ApiKeyHeaderHandler(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.TryAddWithoutValidation("X-API-KEY", _apiKey);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
