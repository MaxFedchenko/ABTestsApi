namespace ABTestsApi.Models.Services
{
    public class ExperimentOption
    {
        public required int Id { get; set; }
        public required string Value { get; set; }
        public required decimal Chance { get; set; }
    }
}
