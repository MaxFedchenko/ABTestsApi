namespace ABTestsApi.Models.Services
{
    public interface IExperimentValueProviderService
    {
        Task<string> Get(string deviceToken, string experimentName);
    }
}