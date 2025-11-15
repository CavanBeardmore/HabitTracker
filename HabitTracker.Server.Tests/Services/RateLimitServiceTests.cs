using HabitTracker.Server.Repository;
using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.Tests.Services
{
    public class RateLimitServiceTests
    {
        private readonly Mock<IRateLimitRepository> _mockRateLimitRepository;
        private readonly Mock<ILogger<RateLimitService>> _mockLogger;
        private readonly RateLimitService _rateLimitService;
        private readonly ITestOutputHelper _output;

        public RateLimitServiceTests(ITestOutputHelper output)
        {
            _mockLogger = new Mock<ILogger<RateLimitService>>();
            _mockRateLimitRepository = new Mock<IRateLimitRepository>();
            _rateLimitService = new RateLimitService(_mockRateLimitRepository.Object, _mockLogger.Object);
            _output = output;
        }

        [Fact]
        public void HasIpAddressBeenLimited_ReturnsFalseWhenDoesntExist()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns((Rate)null);

            var result = _rateLimitService.HasIpAddressBeenLimited(ipAddress);

            Assert.False(result);
        }

        [Fact]
        public void HasIpAddressBeenLimited_ReturnsFalseRateLimitHasExpired()
        {
            string ipAddress = "127.0.0.1";

            DateTime date = DateTime.UtcNow;

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 1, date.AddYears(-1)));

            var result = _rateLimitService.HasIpAddressBeenLimited(ipAddress);
            _output.WriteLine($"{result}");

            Assert.False(result);
        }

        [Fact]
        public void HasIpAddressBeenLimited_ReturnsFalseRateCountDoesntExceedLimit()
        {
            string ipAddress = "127.0.0.1";

            DateTime date = DateTime.UtcNow;

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 1, date.AddDays(1)));

            var result = _rateLimitService.HasIpAddressBeenLimited(ipAddress);
            _output.WriteLine($"{result}");
            Assert.False(result);
        }

        [Fact]
        public void HasIpAddressBeenLimited_ReturnsTrueRateIsntExpiredAndCountExceedsLimit()
        {
            string ipAddress = "127.0.0.1";

            DateTime date = DateTime.UtcNow;

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 550, date.AddDays(1)));

            var result = _rateLimitService.HasIpAddressBeenLimited(ipAddress);
            Assert.True(result);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldCreateRateIfRateDoesntExist()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns((Rate)null);
            _mockRateLimitRepository.Setup(repository => repository.AddRate(1, ipAddress, It.IsAny<DateTime>())).Returns(new Rate(ipAddress, 1, DateTime.UtcNow));

            _rateLimitService.CheckRateLimitForIpAddress(ipAddress);

            _mockRateLimitRepository.Verify(repository => repository.GetRate(It.IsAny<string>()), Times.Once);
            _mockRateLimitRepository.Verify(repository => repository.AddRate(1, ipAddress, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldThrowAppExceptionIfCreatingRateFails()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns((Rate)null);
            _mockRateLimitRepository.Setup(repository => repository.AddRate(1, ipAddress, It.IsAny<DateTime>())).Returns((Rate)null);

            Assert.Throws<AppException>(() => _rateLimitService.CheckRateLimitForIpAddress(ipAddress));

            _mockRateLimitRepository.Verify(repository => repository.GetRate(It.IsAny<string>()), Times.Once);
            _mockRateLimitRepository.Verify(repository => repository.AddRate(1, ipAddress, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldUpdateRateCountAndTtlIfRateHasExpired()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 1, DateTime.UtcNow.AddDays(-1)));
            _mockRateLimitRepository.Setup(repository => repository.UpdateRateCountAndTtl(1, ipAddress, It.IsAny<DateTime>())).Returns(true);

            _rateLimitService.CheckRateLimitForIpAddress(ipAddress);

            _mockRateLimitRepository.Verify(repository => repository.GetRate(It.IsAny<string>()), Times.Once);
            _mockRateLimitRepository.Verify(repository => repository.AddRate(1, ipAddress, It.IsAny<DateTime>()), Times.Never);
            _mockRateLimitRepository.Verify(repository => repository.UpdateRateCountAndTtl(1, ipAddress, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldThrowAppExceptionIfUpdateRateCountAndTtlFails()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 1, DateTime.UtcNow.AddDays(-1)));
            _mockRateLimitRepository.Setup(repository => repository.UpdateRateCountAndTtl(1, ipAddress, It.IsAny<DateTime>())).Returns(false);

            Assert.Throws<AppException>(() => _rateLimitService.CheckRateLimitForIpAddress(ipAddress));

            _mockRateLimitRepository.Verify(repository => repository.GetRate(It.IsAny<string>()), Times.Once);
            _mockRateLimitRepository.Verify(repository => repository.AddRate(1, ipAddress, It.IsAny<DateTime>()), Times.Never);
            _mockRateLimitRepository.Verify(repository => repository.UpdateRateCountAndTtl(1, ipAddress, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldCallUpdateRateCountAndTtlAndThrowTooManyRequestsExceptionIfCountExceedsLimit()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 510, DateTime.UtcNow.AddDays(1)));
            _mockRateLimitRepository.Setup(repository => repository.UpdateRateCountAndTtl(510, ipAddress, It.IsAny<DateTime>())).Returns(true);

            Assert.Throws<TooManyRequestsException>(() => _rateLimitService.CheckRateLimitForIpAddress(ipAddress));

            _mockRateLimitRepository.Verify(repository => repository.GetRate(It.IsAny<string>()), Times.Once);
            _mockRateLimitRepository.Verify(repository => repository.AddRate(1, ipAddress, It.IsAny<DateTime>()), Times.Never);
            _mockRateLimitRepository.Verify(repository => repository.UpdateRateCountAndTtl(510, ipAddress, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldThrowAppExceptionIfUpdateRateCountAndTtlFailsWhenRateCountExceedsLimit()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 510, DateTime.UtcNow.AddDays(1)));
            _mockRateLimitRepository.Setup(repository => repository.UpdateRateCountAndTtl(510, ipAddress, It.IsAny<DateTime>())).Returns(false);

            Assert.Throws<AppException>(() => _rateLimitService.CheckRateLimitForIpAddress(ipAddress));

            _mockRateLimitRepository.Verify(repository => repository.GetRate(It.IsAny<string>()), Times.Once);
            _mockRateLimitRepository.Verify(repository => repository.AddRate(1, ipAddress, It.IsAny<DateTime>()), Times.Never);
            _mockRateLimitRepository.Verify(repository => repository.UpdateRateCountAndTtl(510, ipAddress, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldCallUpdateRateCount()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 1, DateTime.UtcNow.AddDays(1)));
            _mockRateLimitRepository.Setup(repository => repository.UpdateRateCount(2, ipAddress)).Returns(true);

            _rateLimitService.CheckRateLimitForIpAddress(ipAddress);

            _mockRateLimitRepository.Verify(repository => repository.GetRate(It.IsAny<string>()), Times.Once);
            _mockRateLimitRepository.Verify(repository => repository.AddRate(It.IsAny<uint>(), ipAddress, It.IsAny<DateTime>()), Times.Never);
            _mockRateLimitRepository.Verify(repository => repository.UpdateRateCountAndTtl(It.IsAny<uint>(), ipAddress, It.IsAny<DateTime>()), Times.Never);
            _mockRateLimitRepository.Verify(repository => repository.UpdateRateCount(It.IsAny<uint>(), ipAddress), Times.Once);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldThrowAppExceptionWhenUpdateRateCountFails()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Returns(new Rate(ipAddress, 1, DateTime.UtcNow.AddDays(1)));
            _mockRateLimitRepository.Setup(repository => repository.UpdateRateCount(2, ipAddress)).Returns(false);

            Assert.Throws<AppException>(() => _rateLimitService.CheckRateLimitForIpAddress(ipAddress));

            _mockRateLimitRepository.Verify(repository => repository.GetRate(It.IsAny<string>()), Times.Once);
            _mockRateLimitRepository.Verify(repository => repository.AddRate(It.IsAny<uint>(), ipAddress, It.IsAny<DateTime>()), Times.Never);
            _mockRateLimitRepository.Verify(repository => repository.UpdateRateCountAndTtl(It.IsAny<uint>(), ipAddress, It.IsAny<DateTime>()), Times.Never);
            _mockRateLimitRepository.Verify(repository => repository.UpdateRateCount(It.IsAny<uint>(), ipAddress), Times.Once);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldThrowAppExceptionWhenAnExceptionOccurs()
        {
            string ipAddress = "127.0.0.1";

            _mockRateLimitRepository.Setup(repository => repository.GetRate(ipAddress)).Throws<Exception>(() => throw new Exception("test"));

            Assert.Throws<AppException>(() => _rateLimitService.CheckRateLimitForIpAddress(ipAddress));
        }
    }
}
