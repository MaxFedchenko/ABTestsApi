using ABTestsApi.Models.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ABTestsApi.Common.AuthenticationHandlers
{
    // AuthenticationHandler to authenticate device and register it if it is new
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

            // Extract the device-token value from the query
            if (queryParameters.TryGetValue(tokenParamName, out var queryValues) &&
                !string.IsNullOrWhiteSpace(deviceToken = queryValues.FirstOrDefault()))
            {
                // Check if device is registered
                if (!await _deviceService.IsRegistered(deviceToken))
                {
                    // If isn't, register it
                    await _deviceService.Register(deviceToken);
                }

                // Add device token claim to identity
                var claim = new Claim(DeviceTokenAuthenticationClaimTypes.DeviceToken, deviceToken);
                var claimsIdentity = new ClaimsIdentity(new[] { claim }, Scheme.Name);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                
                return AuthenticateResult.Success(ticket);
            }

            // If device-token is not found in query or is invalid, return failure
            return AuthenticateResult.Fail("Device token wasn't provided in query parameters");
        }
    }
}
