using ABTestsApi.DataAccess;
using Microsoft.Extensions.Caching.Memory;

namespace ABTestsApi.Models.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _devRepository;
        private readonly IMemoryCache _memoryCache;

        public DeviceService(IDeviceRepository devRepository,
                             IMemoryCache memoryCache)
        {
            _devRepository = devRepository;
            _memoryCache = memoryCache;
        }

        // Cache key for a device
        private static string CacheKey(string token) => $"Device:{token}";

        private async Task<DataAccess.Device?> GetDevice(string token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            // Check cache for the device
            if (!_memoryCache.TryGetValue(CacheKey(token), out DataAccess.Device? device))
            {
                // If cache missed, get it from the DB
                device = await _devRepository.GetByToken(token);
                if (device is not null)
                {
                    // If the device is in the DB, cache it
                    _memoryCache.Set(CacheKey(token), device);
                }
            }

            return device;
        }

        public async Task<Device> Find(string token)
        {
            var device = await GetDevice(token);

            if (device is null)
                throw new ArgumentException("No device with this token is registered", nameof(token));

            return new Device
            {
                Id = device.Id,
                Token = device.Token!,
                CreationTime = device.CreationTime
            };
        }
        public async Task<bool> IsRegistered(string token)
        {
            var device = await GetDevice(token);
            return device is not null;
        }
        public async Task<DateTime> WhenRegistered(string token)
        {
            var device = await Find(token);
            return device.CreationTime;
        }
        public async Task Register(string token)
        {
            var device = new DataAccess.Device { Token = token };
            await _devRepository.Create(device);

            // Cache the device on successful creation
            _memoryCache.Set(CacheKey(token), device);
        }
    }
}
