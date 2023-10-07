using ABTestsApi.Common.Constants;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ABTestsApi.DataAccess
{
    public class DeviceExperimentOptionRepository : BaseRepository, IDeviceExperimentOptionRepository
    {
        public DeviceExperimentOptionRepository(IDatabaseManager databaseManager) : base(databaseManager) { }

        public async Task Create(DeviceExperimentOption deviceExptOpt)
        {
            if (deviceExptOpt is null)
                throw new ArgumentNullException(nameof(deviceExptOpt));

            await DatabaseManager.ExecuteNonQuery(ProcedureNames.CreateDeviceExptOpt, new[]
            {
                new SqlParameter("@device_id", deviceExptOpt.DeviceId),
                new SqlParameter("@option_id", deviceExptOpt.ExperimentOptionId)
            });
        }
    }
}
