namespace ABTestsApi.Models.Services
{
    public interface IDeviceService
    {
        Task<Device> Find(string token);
        Task<bool> IsRegistered(string token);
        Task Register(string token);
        Task<DateTime> WhenRegistered(string token);
    }
}