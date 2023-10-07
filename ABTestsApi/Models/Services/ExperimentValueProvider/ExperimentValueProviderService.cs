namespace ABTestsApi.Models.Services
{
    public class ExperimentValueProviderService : IExperimentValueProviderService
    {
        private readonly IExperimentService _experimentService;

        public ExperimentValueProviderService(IExperimentService experimentService)
        {
            _experimentService = experimentService;
        }

        // Select option for device, by option probabilities
        private static ExperimentOption SelectOption(ExperimentOption[] options)
        {
            // Generate a random decimal value in the range [0, 1) 
            decimal randomValue = (decimal)new Random().NextDouble();

            // Initialize cumulative probability
            decimal cumulativeProbability = 0;

            foreach (var category in options)
            {
                // Add the chance of the current option to the cumulative probability
                cumulativeProbability += category.Chance;

                // Check if the random value falls within the option chance range
                if (randomValue < cumulativeProbability)
                    return category;
            }

            throw new InvalidOperationException("Failed to select a category value. Total sum of experiment options probabilities is less than 1");
        }

        public async Task<string> Get(string deviceToken, string experimentName)
        {
            // Get participant's option value
            var value = await _experimentService.GetOptionValueOfDevice(experimentName, deviceToken);
            if (value is not null)
                return value;

            // If isn't a participant, select option to make one
            // Get all experiment options
            var options = await _experimentService.GetOptions(experimentName);
            // Select option
            var selectedOption = SelectOption(options);
            // Make device a participant of experiment with selected option 
            await _experimentService.SetOptionForDevice(experimentName, deviceToken, selectedOption.Id);

            return selectedOption.Value;
        }
    }
}
