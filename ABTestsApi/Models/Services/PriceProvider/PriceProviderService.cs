namespace ABTestsApi.Models.Services
{
    // Provides default price value if a device doesn't participate in an experiment
    public class PriceProviderService : IPriceProviderService
    {
        public decimal Get() => 10;
    }
}
