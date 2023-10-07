using ABTestsApi.Common.Constants;

namespace ABTestsApi.DataAccess
{
    public class ExperimentRepository : BaseRepository, IExperimentRepository
    {
        public ExperimentRepository(IDatabaseManager databaseManager) : base(databaseManager) { }

        public async IAsyncEnumerable<Experiment> GetAll()
        {
            var reader = DatabaseManager.ExecuteReader(ProcedureNames.GetAllExpts);

            await foreach(var row in reader)
            {
                yield return new Experiment
                {
                    Id = row.GetInt32(0),
                    Name = row.GetString(1),
                    CreationTime = row.GetDateTime(2)
                };
            }
        }
    }
}
