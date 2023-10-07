namespace ABTestsApi.DataAccess
{
    public class ExperimentOption
    {
        public int Id { get; set; }
        public string? Value { get; set; }
        public decimal Chance { get; set; }
        public int ExperimentId { get; set; }
    }
}
