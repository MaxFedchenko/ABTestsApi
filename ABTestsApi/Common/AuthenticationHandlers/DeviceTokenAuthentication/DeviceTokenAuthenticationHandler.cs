using ABTestsApi.Models.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ABTestsApi.Common.AuthenticationHandlers
{
    // AuthenticationHandler which authenticates devices and registers them if they are new
    public class DeviceTokenAuthenticationHandler : AuthenticationHandler<DeviceTokenAuthenticationOptions>
    {
        private readonly IDeviceService _deviceService;

        public DeviceTokenAuthenticationHandler(
            IDeviceService deviceService,
            IOptionsMonitor<DeviceTokenAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _deviceService = deviceService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? deviceToken;
            var queryParameters = Context.Request.Query;
            var tokenParamName = !string.IsNullOrWhiteSpace(Options.DeviceTokenQueryParamName) ?
                Options.DeviceTokenQueryParamName :
                throw new InvalidOperationException("DeviceTokenQueryParamName isn't valid");

            // Extract a device-token value from the query
            if (queryParameters.TryGetValue(tokenParamName, out var queryValues) &&
                !string.IsNullOrWhiteSpace(deviceToken = queryValues.FirstOrDefault()))
            {
                // Check if the device is registered
                if (!await _deviceService.IsRegistered(deviceToken))
                {
                    // If isn't, register it
                    await _deviceService.Register(deviceToken);
                }

                // Add the device token claim to the identity
                var claim = new Claim(DeviceTokenAuthenticationClaimTypes.DeviceToken, deviceToken);
                var claimsIdentity = new ClaimsIdentity(new[] { claim }, Scheme.Name);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                
                return AuthenticateResult.Success(ticket);
            }

            // If a device-token is not found in the query or is invalid, return a failure
            return AuthenticateResult.Fail("Device token wasn't provided in query parameters");
        }
    }
}
