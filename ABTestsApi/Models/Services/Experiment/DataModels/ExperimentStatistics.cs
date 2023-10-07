namespace ABTestsApi.Models.Services
{
    public class ExperimentStatistics
    {
        public required string Name { get; set; }
        public required int TotalDevices { get; set; }
        public required IEnumerable<OptionStatistics> Options { get; set; }
    }
    public class OptionStatistics
    {
        public required string Value { get; set; }
        public required decimal Chance { get; set; }
        public required int Devices { get; set; }
    }
}
