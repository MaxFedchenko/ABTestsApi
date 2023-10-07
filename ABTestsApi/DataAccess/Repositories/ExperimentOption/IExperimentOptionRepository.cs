namespace ABTestsApi.DataAccess
{
    public interface IExperimentOptionRepository
    {
        IAsyncEnumerable<ExperimentOption> GetByExperimentId(int experimentId);
        IAsyncEnumerable<ExperimentOptionWithCount> GetWithDeviceCount();
        Task<string?> GetOptionValue(int experimentId, int deviceId);
    }
}