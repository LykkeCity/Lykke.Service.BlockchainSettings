using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Client.Exception;
using Newtonsoft.Json;

namespace Lykke.Service.BlockchainSettings.Client.HttpClientGenerator.DelegatingHandlers
{
    internal class ResponseHandler : DelegatingHandler
    {
        public ResponseHandler()
        {
        }

        /// <inheritdoc />
        /// <exception cref="NotOkException"></exception>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            bool isError = false;
            var result = await base.SendAsync(request, cancellationToken);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                var serializedResponse = await result.Content.ReadAsStringAsync();

                try
                {
                    ErrorResponse error = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(serializedResponse);
                    if (string.IsNullOrEmpty(error?.ErrorMessage))
                        throw new NotOkException((int)result.StatusCode, error.ErrorMessage);

                }
                catch (System.Exception e)
                {
                    throw;
                }
            }

            return result;

        }
    }
}
