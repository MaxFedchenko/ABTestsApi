namespace ABTestsApi.Models.Services
{
    public class ExperimentValueProviderService : IExperimentValueProviderService
    {
        private readonly IExperimentService _experimentService;

        public ExperimentValueProviderService(IExperimentService experimentService)
        {
            _experimentService = experimentService;
        }

        // Selects an option for a device, by option probabilities
        private static ExperimentOption SelectOption(ExperimentOption[] options)
        {
            // Generate a random decimal value in the range of [0, 1) 
            decimal randomValue = (decimal)new Random().NextDouble();

            // Initialize a cumulative probability
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
            // Get the participant option value
            var value = await _experimentService.GetOptionValueOfDevice(experimentName, deviceToken);
            if (value is not null)
                return value;

            // If isn't a participant, select an option to make him one
            // Get all experiment options
            var options = await _experimentService.GetOptions(experimentName);
            // Select an option
            var selectedOption = SelectOption(options);
            // Make device a participant of the experiment with the selected option 
            await _experimentService.SetOptionForDevice(experimentName, deviceToken, selectedOption.Id);

            return selectedOption.Value;
        }
    }
}
