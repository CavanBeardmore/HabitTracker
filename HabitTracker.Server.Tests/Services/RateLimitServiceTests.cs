using HabitTracker.Server.Repository;
using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using HabitTracker.Server.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace HabitTracker.Server.Tests.Services
{
    public class RateLimitServiceTests
    {
        private readonly Mock<ILogger<RateLimitService>> _mockLogger;
        private readonly ITestOutputHelper _output;

        public RateLimitServiceTests(ITestOutputHelper output)
        {
            _mockLogger = new Mock<ILogger<RateLimitService>>();
            _output = output;
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldIncrementRate()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var logger = Mock.Of<ILogger<RateLimitService>>();
            var service = new RateLimitService(memoryCache, logger, ttlLength: 5, rateLimit: 5);

            string ip = "127.0.0.1";

            service.CheckRateLimitForIpAddress(ip);

            service.CheckRateLimitForIpAddress(ip);

            memoryCache.TryGetValue(ip, out uint count);

            Assert.Equal(2u, count);
        }

        [Fact]
        public void CheckRateLimitForIpAddress_ShouldThrowTooManyRequestsException()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var logger = Mock.Of<ILogger<RateLimitService>>();
            var service = new RateLimitService(memoryCache, logger, ttlLength: 5, rateLimit: 2);

            string ip = "127.0.0.1";

            service.CheckRateLimitForIpAddress(ip);
            service.CheckRateLimitForIpAddress(ip);

            Assert.Throws<TooManyRequestsException>(() => service.CheckRateLimitForIpAddress(ip));
        }
    }
}
