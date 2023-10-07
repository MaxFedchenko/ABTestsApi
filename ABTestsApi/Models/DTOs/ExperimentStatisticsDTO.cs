namespace ABTestsApi.Models.DTOs
{
    public class ExperimentStatisticsDTO
    {
        public required string Name { get; set; }
        public required int TotalDevices { get; set; }
        public required IEnumerable<OptionStatisticsDTO> Options { get; set; }
    }
    public class OptionStatisticsDTO
    {
        public required string Value { get; set; }
        public required decimal Chance { get; set; }
        public required int Devices { get; set; }
    }
}
