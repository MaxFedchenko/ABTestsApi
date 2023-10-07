using ABTestsApi.Common.Constants;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ABTestsApi.Swagger
{
    // Adds, on swagger page, device-token query param to all endpoints of experiments controller
    public class DeviceTokenOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor &&
                descriptor.ControllerName == "Experiments")
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = QueryParamNames.DeviceToken,
                    In = ParameterLocation.Query,
                    Required = true,
                });
            }
        }
    }
}
