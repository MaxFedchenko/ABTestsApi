using Microsoft.Extensions.Caching.Memory;
using Moq;
using ABTestsApi.DataAccess;
using ABTestsApi.Models.Services;
using Device = ABTestsApi.Models.Services.Device;

namespace ABTestsApi.Tests
{
    public class ExperimentServiceTests
    {
        private static Device ExpectedDevice = 
            new Device { Id = 1, Token = "ValidToken", CreationTime = DateTime.Now };

        private static Experiment[] ExpectedExperiments = new[]
        {
            new Experiment { Id = 1, Name = "ValidExperimentName", CreationTime = DateTime.Now },
            new Experiment { Id = 2, Name = "ValidExperimentName2", CreationTime = DateTime.Now }
        };

        private static DataAccess.ExperimentOption[] ExpectedOptions = new []
        {
            new DataAccess.ExperimentOption { Id = 1, Value = "Option1", Chance = 0.75m },
            new DataAccess.ExperimentOption { Id = 1, Value = "Option2", Chance = 0.25m },
        };

        private Mock<IDeviceService>? deviceServiceMock;
        private Mock<IExperimentRepository>? expRepositoryMock;
        private Mock<IExperimentOptionRepository>? optRepositoryMock;
        private Mock<IDeviceExperimentOptionRepository>? devExptOptRepositoryMock;

        private IExperimentService CreateExperimentService()
        {
            deviceServiceMock = new Mock<IDeviceService>();
            expRepositoryMock = new Mock<IExperimentRepository>();
            optRepositoryMock = new Mock<IExperimentOptionRepository>();
            devExptOptRepositoryMock = new Mock<IDeviceExperimentOptionRepository>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            deviceServiceMock
                .Setup(devServ => devServ.Find(It.IsAny<string>()))
                .ReturnsAsync((string token) => 
                {
                    if (token == null)
                        throw new ArgumentNullException(nameof(token));
                    if (token == ExpectedDevice.Token)
                        return ExpectedDevice;

                    throw new ArgumentException("Device wasn't found", nameof(token));
                });

            expRepositoryMock.Setup(repo => repo.GetAll())
                .Returns(ExpectedExperiments.ToAsyncEnumerable());

            optRepositoryMock.Setup(repo => repo.GetByExperimentId(It.IsAny<int>()))
                .Returns((int exptId) => 
                {
                    if (exptId == ExpectedExperiments[0].Id)
                        return ExpectedOptions.ToAsyncEnumerable();

                    return Array.Empty<DataAccess.ExperimentOption>().ToAsyncEnumerable();
                });
            optRepositoryMock.Setup(repo => repo.GetOptionValue(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((int exptId, int devId) => 
                {
                    if (exptId == ExpectedExperiments[0].Id && devId == ExpectedDevice.Id)
                        return ExpectedOptions[0].Value;

                    return null;
                });

            devExptOptRepositoryMock.Setup(repo => repo.Create(It.IsAny<DeviceExperimentOption>()));

            return new ExperimentService(
                deviceServiceMock.Object,
                expRepositoryMock.Object,
                optRepositoryMock.Object,
                devExptOptRepositoryMock.Object,
                memoryCache
            );
        }

        [Fact]
        public async Task Exists_ReturnsTrue()
        {
            // Arrange
            var experimentName = ExpectedExperiments[0].Name!;
            var experimentService = CreateExperimentService();

            // Act
            var result = await experimentService.Exists(experimentName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse()
        {
            // Arrange
            var experimentName = "NonexistentExperiment";
            var experimentService = CreateExperimentService();

            // Act
            var result = await experimentService.Exists(experimentName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task WhenStarted_ReturnsCreationTime()
        {
            // Arrange
            var experimentName = ExpectedExperiments[0].Name!;
            var experimentService = CreateExperimentService();

            // Act
            var result = await experimentService.WhenStarted(experimentName);

            // Assert
            Assert.Equal(ExpectedExperiments[0].CreationTime, result);
        }

        [Fact]
        public async Task GetOptions_ReturnsExperimentOptions()
        {
            // Arrange
            var experimentName = ExpectedExperiments[0].Name!;
            var experimentService = CreateExperimentService();

            // Act
            var result = await experimentService.GetOptions(experimentName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ExpectedOptions.Length, result.Length);
            Assert.Equal(ExpectedOptions.Select(o => o.Id), result.Select(o => o.Id));
        }

        [Fact]
        public async Task GetOptions_ReturnsEmptyArray()
        {
            // Arrange
            var experimentName = ExpectedExperiments[1].Name!;
            var experimentService = CreateExperimentService();

            // Act
            var result = await experimentService.GetOptions(experimentName);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetOptionValueOfDevice_ReturnsOptionValue()
        {
            // Arrange
            var experimentName = ExpectedExperiments[0].Name!;
            var deviceToken = ExpectedDevice.Token;
            var experimentService = CreateExperimentService();

            // Act
            var result = await experimentService.GetOptionValueOfDevice(experimentName, deviceToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ExpectedOptions[0].Value, result);
        }

        [Fact]
        public async Task GetOptionValueOfDevice_ReturnsNull()
        {
            // Arrange
            var experimentName = ExpectedExperiments[1].Name!;
            var deviceToken = ExpectedDevice.Token;
            var experimentService = CreateExperimentService();

            // Act
            var result = await experimentService.GetOptionValueOfDevice(experimentName, deviceToken);

            // Assert
            optRepositoryMock?.Verify(m => m.GetOptionValue(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public async Task SetOptionForDevice_CreatesOption()
        {
            // Arrange
            var experimentName = ExpectedExperiments[0].Name!;
            var deviceToken = ExpectedDevice.Token;
            var optionId = 1;
            var experimentService = CreateExperimentService();

            // Act
            await experimentService.SetOptionForDevice(experimentName, deviceToken, optionId);

            // Assert
            devExptOptRepositoryMock!.Verify(m =>
                m.Create(It.Is<DeviceExperimentOption>(deo => 
                    deo.ExperimentOptionId == optionId && 
                    deo.DeviceId == ExpectedDevice.Id)), 
                    Times.Once);
        }
    }
}
