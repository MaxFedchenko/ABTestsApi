namespace ABTestsApi.Models.Services
{
    // Provides default price value if device doesn't participate in experiment
    public class PriceProviderService : IPriceProviderService
    {
        public decimal Get() => 10;
    }
}
