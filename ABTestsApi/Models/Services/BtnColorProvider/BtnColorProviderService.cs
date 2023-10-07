namespace ABTestsApi.Models.Services
{
    // Provides default button color value if device doesn't participate in experiment
    public class BtnColorProviderService : IBtnColorProviderService
    {
        public string Get() => "#FF0000";
    }
}
