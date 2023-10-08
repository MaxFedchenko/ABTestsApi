namespace ABTestsApi.Models.Services
{
    // Provides default button color value if a device doesn't participate in an experiment
    public class BtnColorProviderService : IBtnColorProviderService
    {
        public string Get() => "#FF0000";
    }
}
