using ABTestsApi.Common.Constants;
using Microsoft.Data.SqlClient;

namespace ABTestsApi.DataAccess
{
    public class ExperimentOptionRepository : BaseRepository, IExperimentOptionRepository
    {
        public ExperimentOptionRepository(IDatabaseManager databaseManager) : base(databaseManager) { }

        public async IAsyncEnumerable<ExperimentOption> GetByExperimentId(int experimentId)
        {
            var reader = DatabaseManager.ExecuteReader(ProcedureNames.GetOptsByExptId, new[]
            {
                new SqlParameter("@experiment_id", experimentId)
            });

            await foreach(var row in reader)
            {
                yield return new ExperimentOption
                {
                    Id = row.GetInt32(0),
                    Value = row.GetString(1),
                    Chance = row.GetDecimal(2),
                    ExperimentId = experimentId
                };
            }
        }
        public async IAsyncEnumerable<ExperimentOptionWithCount> GetWithDeviceCount()
        {
            var reader = DatabaseManager.ExecuteReader(ProcedureNames.GetOptsWithDeviceCount);

            await foreach (var row in reader)
            {
                yield return new ExperimentOptionWithCount
                {
                    Id = row.GetInt32(0),
                    Value = row.GetString(1),
                    Chance = row.GetDecimal(2),
                    ExperimentId = row.GetInt32(3),
                    DevicesCount = row.GetInt32(4)
                };
            }
        }
        public async Task<string?> GetOptionValue(int experimentId, int deviceId)
        {
            var value = await DatabaseManager.ExecuteScalar<string?>(ProcedureNames.GetOptValueByDeviceIdExptId, new[]
            {
                new SqlParameter("@device_id", deviceId),
                new SqlParameter("@experiment_id", experimentId)
            });

            return value;
        }
    }
}
