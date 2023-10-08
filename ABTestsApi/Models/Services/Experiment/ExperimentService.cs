using ABTestsApi.DataAccess;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace ABTestsApi.Models.Services
{
    public class ExperimentService : IExperimentService
    {
        private readonly IDeviceService _deviceService;
        private readonly IExperimentRepository _expRepository;
        private readonly IExperimentOptionRepository _optRepository;
        private readonly IDeviceExperimentOptionRepository _devExptOptRepository;
        private readonly IMemoryCache _memoryCache;

        private readonly static SemaphoreSlim _cacheLock = new SemaphoreSlim(1);

        public ExperimentService(IDeviceService deviceService,
                                 IExperimentRepository expRepository,
                                 IExperimentOptionRepository optRepository,
                                 IDeviceExperimentOptionRepository devExptOptRepository,
                                 IMemoryCache memoryCache)
        {
            _deviceService = deviceService;
            _expRepository = expRepository;
            _optRepository = optRepository;
            _devExptOptRepository = devExptOptRepository;
            _memoryCache = memoryCache;
        }

        // Cache key for all experiments
        private static string CacheExptsKey() => "Experiments";
        // Cache key for experiment options
        private static string CacheOptKey(int id) => $"Options:{id}";
        // Cache key for an experiment option value of a device
        private static string CacheDevOptKey(int id, int deviceId) => $"DevOpt:{id}_{deviceId}";

        private async Task<Experiment[]> GetAllExperiments()
        {
            Experiment[] experiments;

            // Lock to prevent cache stampede
            await _cacheLock.WaitAsync();

            try
            {
                if (!_memoryCache.TryGetValue(CacheExptsKey(), out experiments!))
                {
                    // If cache missed, get experiments from the DB 
                    experiments = await _expRepository.GetAll().ToArrayAsync();

                    // Cache experiments 
                    // Cache is cleared every 30 seconds. This way it can be updated with new experiments
                    _memoryCache.Set(CacheExptsKey(), experiments,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));
                }

                return experiments;
            }
            finally
            {
                _cacheLock.Release();
            }
        }
        private async Task<Experiment?> GetExperiment(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var experiments = await GetAllExperiments();
            var experiment = experiments.FirstOrDefault(e => e.Name == name);

            return experiment;
        }
        private async Task<Experiment> FindExperiment(string name)
        {
            var experiment = await GetExperiment(name);

            if (experiment is null)
                throw new ArgumentException("No such experiment exists", nameof(name));

            return experiment;
        }

        private async Task<DataAccess.ExperimentOption[]> GetExperimentOptions(string name)
        {
            // Find the experiment and get its id
            var experiment = await FindExperiment(name);
            int id = experiment.Id;

            DataAccess.ExperimentOption[] options;

            // Return experiment options on cache hit
            if (_memoryCache.TryGetValue(CacheOptKey(id), out options!))
                return options;

            // Get experiment options from the DB on cache miss
            options = await _optRepository.GetByExperimentId(id).ToArrayAsync();

            // On successful receipt of experiment options, cache them
            _memoryCache.Set(CacheOptKey(id), options);

            return options;
        }
        private async Task<DataAccess.ExperimentOption> FindOption(string name, int optionId) 
        {
            var options = await GetExperimentOptions(name);
            return options.First(o => o.Id == optionId);
        }

        public async Task<bool> Exists(string experiment) 
        {
            return await GetExperiment(experiment) is not null;
        }
        public async Task<DateTime> WhenStarted(string experiment)
        {
            var expt = await FindExperiment(experiment);
            return expt.CreationTime;
        }
        public async Task<ExperimentOption[]> GetOptions(string experiment)
        {
            var options = await GetExperimentOptions(experiment);
            return options.Select(o => new ExperimentOption
            {
                Id = o.Id,
                Value = o.Value!,
                Chance = o.Chance,
            }).ToArray();
        }

        // Gets the experiment option that was assigned to the device. If the device isn't a participant of the experiment yet, it returns null
        public async Task<string?> GetOptionValueOfDevice(string experiment, string deviceToken)
        {
            var deviceTask = _deviceService.Find(deviceToken);
            var exptTask = FindExperiment(experiment);

            await Task.WhenAll(deviceTask, exptTask);
            var device = await deviceTask;
            var expt = await exptTask;

            // Try get the value from cache 
            if (!_memoryCache.TryGetValue(CacheDevOptKey(expt.Id, device.Id), out string? value))
            {
                // If cache missed, try get it from the DB
                value = await _optRepository.GetOptionValue(expt.Id, device.Id);

                // If present in the DB, cache the value
                if (value is not null)
                    _memoryCache.Set(CacheDevOptKey(expt.Id, device.Id), value);
            }

            return value;
        }
        // Makes the device a participant of the experiment by setting the option value for it
        public async Task SetOptionForDevice(string experiment, string deviceToken, int optionId)
        {
            var device = await _deviceService.Find(deviceToken);

            await _devExptOptRepository.Create(new DeviceExperimentOption
            {
                DeviceId = device.Id,
                ExperimentOptionId = optionId
            });

            var exptTask = FindExperiment(experiment);
            var optionTask = FindOption(experiment, optionId);

            await Task.WhenAll(exptTask, optionTask);
            var expt = await exptTask;
            var option = await optionTask;

            // Cache the option value that was set for the device
            _memoryCache.Set(CacheDevOptKey(expt.Id, device.Id), option.Value);
        }

        public async Task<IEnumerable<ExperimentStatistics>> GetStatistics()
        {
            OptionStatistics MapOpt(ExperimentOptionWithCount opt) =>
                new OptionStatistics
                {
                    Value = opt.Value!,
                    Chance = opt.Chance,
                    Devices = opt.DevicesCount
                };
            ExperimentStatistics Map(Experiment expt, IEnumerable<ExperimentOptionWithCount> options) 
            {
                var optStat = options.Select(MapOpt).ToArray();

                return new ExperimentStatistics
                {
                    Name = expt.Name!,
                    TotalDevices = optStat.Sum(o => o.Devices),
                    Options = optStat
                };
            }

            // Get experiments and options
            var optionsTask = _optRepository.GetWithDeviceCount().ToArrayAsync().AsTask();
            var experimentsTask = GetAllExperiments();

            await Task.WhenAll(optionsTask, experimentsTask);
            var options = await optionsTask;
            var experiments = await experimentsTask;

            // Join experiments and options
            return experiments.GroupJoin(options, e => e.Id, o => o.ExperimentId, Map);
        }
    }
}
