using Microsoft.AspNetCore.Mvc.Filters;
using ABTestsApi.Common.AuthenticationHandlers;
using ABTestsApi.Models.Services;
using Microsoft.AspNetCore.Mvc;
using ABTestsApi.Models.DTOs;

namespace ABTestsApi.Common.ActionFilters
{
    // Checks if a device meets conditions to participate in an experiment
    // If does, delegates value retrieval to the experiment service, otherwise, a controller action handles it
    public class ExperimentFilterAttribute : ActionFilterAttribute
    {
        private readonly IExperimentService _experimentService;
        private readonly IDeviceService _deviceService;
        private readonly IExperimentValueProviderService _experimentValueProviderService;

        private readonly string _experimentName;

        public ExperimentFilterAttribute(
            IExperimentService experimentService, 
            IDeviceService deviceService,
            IExperimentValueProviderService experimentValueProviderService,
            string experimentName) 
        {
            _experimentService = experimentService;
            _deviceService = deviceService;
            _experimentValueProviderService = experimentValueProviderService;

            _experimentName = experimentName;
        }

        private async Task<IActionResult> GetExperimentResult(string deviceToken)
        {
            var value = await _experimentValueProviderService.Get(deviceToken, _experimentName);
            return new OkObjectResult(new KeyValueDTO { Key = _experimentName, Value = value });
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get the device token claims
            var deviceToken = context.HttpContext.User.Claims
                .First(c => c.Type == DeviceTokenAuthenticationClaimTypes.DeviceToken)
                .Value;

            if (await _experimentService.Exists(_experimentName))
            {
                var deviceRegistered = await _deviceService.WhenRegistered(deviceToken);
                var experimentStarted = await _experimentService.WhenStarted(_experimentName);

                // If the experiment was created before device registration, the device can participate
                if (deviceRegistered > experimentStarted)
                {
                    context.Result = await GetExperimentResult(deviceToken);
                    return;
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
