using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainSettings.Shared.Security;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Service.BlockchainSettings.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly ApiKeyAccessType _apiKeyAccessType;
        public const string ApiKeyHeaderName = "X-API-KEY";

        public ApiKeyAuthorizeAttribute(ApiKeyAccessType apiKeyAccessType)
        {
            _apiKeyAccessType = apiKeyAccessType;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var accessTokenService = (IAccessTokenService)context.HttpContext.RequestServices.GetService(typeof(IAccessTokenService));

            if (context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var values))
            {
                bool isAllowed = values.Any(key =>
                {
                    var accessType = accessTokenService.GetTokenAccess(key);

                    return (_apiKeyAccessType & accessType) != 0; //flags
                });

                if (isAllowed)
                    return base.OnActionExecutionAsync(context, next);
            }

            var errorResponse = new ErrorResponse()
            {
                ErrorMessage = $"Api Key with {_apiKeyAccessType} access should be provided"
            };

            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse)
            };

            return Task.FromResult(0);
        }
    }
}
