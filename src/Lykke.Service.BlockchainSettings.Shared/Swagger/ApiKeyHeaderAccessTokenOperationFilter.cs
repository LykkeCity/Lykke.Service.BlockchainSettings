﻿using System.Collections.Generic;
using System.Linq;
using Lykke.Service.BlockchainSettings.Shared.Attributes;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.BlockchainSettings.Shared.Swagger
{
    public class ApiKeyHeaderAccessTokenOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isTokenAccess = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is ApiKeyAuthorizeAttribute);
            if (isTokenAccess)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = ApiKeyAuthorizeAttribute.ApiKeyHeaderName,
                    In = "header",
                    Description = "Api key",
                    Required = true,
                    Type = "string"
                });
            }
        }
    }
}
