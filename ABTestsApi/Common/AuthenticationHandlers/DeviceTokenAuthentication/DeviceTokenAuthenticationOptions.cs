using Microsoft.AspNetCore.Authentication;

namespace ABTestsApi.Common.AuthenticationHandlers
{
    public class DeviceTokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string? DeviceTokenQueryParamName { get; set; }
    }
}
