using ABTestsApi.Common.Constants;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ABTestsApi.DataAccess
{
    public class DeviceRepository : BaseRepository, IDeviceRepository
    {
        public DeviceRepository(IDatabaseManager databaseManager) : base(databaseManager) { }

        public async Task<Device?> GetByToken(string token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var reader = DatabaseManager.ExecuteReader(ProcedureNames.GetDeviceByToken, new[]
            {
                new SqlParameter("@token", token)
            });

            Device? device = null;
            await foreach (var row in reader)
            {
                device = new Device
                {
                    Id = row.GetInt32(0),
                    Token = token,
                    CreationTime = row.GetDateTime(1),
                };
                break;
            }

            return device;
        }
        public async Task Create(Device device)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            var returnParam = new SqlParameter
            {
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.ReturnValue
            };
            var creationTimeParam = new SqlParameter
            {
                ParameterName = "@creation_time",
                SqlDbType = SqlDbType.DateTime2,
                Direction = ParameterDirection.Output
            };
            await DatabaseManager.ExecuteNonQuery(ProcedureNames.CreateDevice, new[]
            {
                new SqlParameter("@token", device.Token),
                returnParam,
                creationTimeParam
            });

            device.Id = (int)returnParam.Value;
            device.CreationTime = (DateTime)creationTimeParam.Value;
        }
    }
}
