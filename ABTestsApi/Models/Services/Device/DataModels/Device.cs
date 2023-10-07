namespace ABTestsApi.Models.Services
{
    public class Device
    {
        public required int Id { get; set; }
        public required string Token { get; set; }
        public required DateTime CreationTime { get; set; }
    }
}
