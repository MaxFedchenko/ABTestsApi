using Microsoft.Extensions.Caching.Memory;
using Moq;
using ABTestsApi.DataAccess;
using ABTestsApi.Models.Services;

namespace ABTestsApi.Tests
{
    public class DeviceServiceTests
    {
        [Fact]
        public async Task Find_ReturnsDevice()
        {
            // Arrange
            var token = "ValidToken";
            var expectedDevice = new DataAccess.Device
            {
                Id = 1,
                Token = token,
                CreationTime = DateTime.Now
            };

            var devRepositoryMock = new Mock<IDeviceRepository>();
            devRepositoryMock.Setup(repo => repo.GetByToken(token))
                .ReturnsAsync(expectedDevice);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var deviceService = new DeviceService(devRepositoryMock.Object, memoryCache);

            // Act
            var result = await deviceService.Find(token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDevice.Id, result.Id);
            Assert.Equal(expectedDevice.Token, result.Token);
            Assert.Equal(expectedDevice.CreationTime, result.CreationTime);
        }

        [Fact]
        public async Task Find_ThrowsArgumentException()
        {
            // Arrange
            var token = "NonexistentToken";

            var devRepositoryMock = new Mock<IDeviceRepository>();
            devRepositoryMock.Setup(repo => repo.GetByToken(token))
                .ReturnsAsync((DataAccess.Device?)null);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var deviceService = new DeviceService(devRepositoryMock.Object, memoryCache);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(() => deviceService.Find(token));
        }

        [Fact]
        public async Task IsRegistered_ReturnsTrue()
        {
            // Arrange
            var token = "ValidToken";

            var devRepositoryMock = new Mock<IDeviceRepository>();
            devRepositoryMock.Setup(repo => repo.GetByToken(token))
                .ReturnsAsync(new DataAccess.Device());

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var deviceService = new DeviceService(devRepositoryMock.Object, memoryCache);

            // Act
            var result = await deviceService.IsRegistered(token);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsRegistered_ReturnsFalse()
        {
            // Arrange
            var token = "NonexistentToken";

            var devRepositoryMock = new Mock<IDeviceRepository>();
            devRepositoryMock.Setup(repo => repo.GetByToken(token))
                .ReturnsAsync((DataAccess.Device?)null);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var deviceService = new DeviceService(devRepositoryMock.Object, memoryCache);

            // Act
            var result = await deviceService.IsRegistered(token);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task WhenRegistered_ReturnsCreationTime()
        {
            // Arrange
            var token = "ValidToken";
            var expectedCreationTime = DateTime.Now;

            var devRepositoryMock = new Mock<IDeviceRepository>();
            devRepositoryMock.Setup(repo => repo.GetByToken(token))
                .ReturnsAsync(new DataAccess.Device
                {
                    Id = 1,
                    Token = token,
                    CreationTime = expectedCreationTime
                });

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var deviceService = new DeviceService(devRepositoryMock.Object, memoryCache);

            // Act
            var result = await deviceService.WhenRegistered(token);

            // Assert
            Assert.Equal(expectedCreationTime, result);
        }

        [Fact]
        public async Task Register_CreatesDeviceAndCachesIt()
        {
            // Arrange
            var token = "NewToken";

            var devRepositoryMock = new Mock<IDeviceRepository>();
            devRepositoryMock.Setup(repo => repo.Create(It.IsAny<DataAccess.Device>()));

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var deviceService = new DeviceService(devRepositoryMock.Object, memoryCache);

            // Act
            await deviceService.Register(token);

            // Assert
            devRepositoryMock.Verify(repo => repo.Create(It.IsAny<DataAccess.Device>()), Times.Once);
            Assert.True(memoryCache.TryGetValue($"Device:{token}", out DataAccess.Device? _));
        }
    }
}
