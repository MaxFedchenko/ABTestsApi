using Moq;
using ABTestsApi.Models.Services;

namespace ABTestsApi.Tests
{
    public class ExperimentValueProviderServiceTests
    {
        [Fact]
        public async Task Get_ReturnsExistingOptionValue()
        {
            // Arrange
            var deviceToken = "DeviceToken";
            var experimentName = "ExperimentName";
            var expectedValue = "ExistingValue";

            var experimentServiceMock = new Mock<IExperimentService>();
            experimentServiceMock
                .Setup(service => service.GetOptionValueOfDevice(experimentName, deviceToken))
                .ReturnsAsync(expectedValue);

            var service = new ExperimentValueProviderService(experimentServiceMock.Object);

            // Act
            var result = await service.Get(deviceToken, experimentName);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public async Task Get_SelectsOptionAndReturnsNewValue()
        {
            // Arrange
            var deviceToken = "DeviceToken";
            var experimentName = "ExperimentName";

            string? existingValue = null;

            var options = new[]
            {
                new ExperimentOption { Id = 1, Value = "Option1", Chance = 0.3m },
                new ExperimentOption { Id = 2, Value = "Option2", Chance = 0.5m },
                new ExperimentOption { Id = 3, Value = "Option3", Chance = 0.2m }
            };

            var experimentServiceMock = new Mock<IExperimentService>();
            experimentServiceMock
                .Setup(service => service.GetOptionValueOfDevice(experimentName, deviceToken))
                .ReturnsAsync(existingValue);
            experimentServiceMock
                .Setup(service => service.GetOptions(experimentName))
                .ReturnsAsync(options);

            var service = new ExperimentValueProviderService(experimentServiceMock.Object);

            // Act
            var result = await service.Get(deviceToken, experimentName);

            // Assert
            Assert.Contains(result, options.Select(option => option.Value));
        }
    }
}
