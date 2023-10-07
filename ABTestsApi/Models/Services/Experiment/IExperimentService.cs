namespace ABTestsApi.Models.Services
{
    public interface IExperimentService
    {
        Task<bool> Exists(string experiment);
        Task<ExperimentOption[]> GetOptions(string experiment);
        Task<DateTime> WhenStarted(string experiment);
        Task<string?> GetOptionValueOfDevice(string experiment, string deviceToken);
        Task SetOptionForDevice(string experiment, string deviceToken, int optionId);
        Task<IEnumerable<ExperimentStatistics>> GetStatistics();
    }
}