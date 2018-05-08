using Lykke.Service.BlockchainSettings.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Service.BlockchainSettings.Attributes
{
    public class CheckModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                context.Result = new BadRequestObjectResult(context.ModelState.ToErrorResponse());
            }

            base.OnActionExecuting(context);
        }
    }
}
