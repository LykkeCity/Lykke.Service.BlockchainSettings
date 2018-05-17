using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSettings.Shared.Security;
using Lykke.Service.BlockchainSettings.Shared.Settings.ServiceSettings;
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

            context.Result = new UnauthorizedResult();

            return Task.FromResult(0);
        }
    }
}
