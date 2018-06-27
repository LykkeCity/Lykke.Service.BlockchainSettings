using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Client.Exception;
using Lykke.Service.BlockchainSettings.Tests.DelegatingHandlers;
using Newtonsoft.Json;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers
{
    //Used in tests only to redirect http requests to the test fixture server
    internal class RequestInterceptorHandler : DelegatingHandler
    {
        private readonly HttpClient _httpClient;

        public RequestInterceptorHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var clonedRequest = await request.CloneAsync();
            var response = await _httpClient.SendAsync(clonedRequest, cancellationToken);

            return response;
        }
    }
}
