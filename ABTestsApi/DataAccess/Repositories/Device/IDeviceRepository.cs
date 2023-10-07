namespace ABTestsApi.DataAccess
{
    public interface IDeviceRepository
    {
        Task<Device?> GetByToken(string token);
        Task Create(Device device);
    }
}