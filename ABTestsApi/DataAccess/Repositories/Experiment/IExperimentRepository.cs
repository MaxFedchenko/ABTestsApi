namespace ABTestsApi.DataAccess
{
    public interface IExperimentRepository
    {
        IAsyncEnumerable<Experiment> GetAll();
    }
}