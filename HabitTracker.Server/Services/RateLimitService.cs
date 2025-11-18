using HabitTracker.Server.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace HabitTracker.Server.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly uint _ttlLength;
        private readonly uint _rateLimit;

        private readonly ILogger<RateLimitService> _logger;
        private readonly IMemoryCache _cache;

        public RateLimitService(IMemoryCache cache, ILogger<RateLimitService> logger, uint ttlLength = 5, uint rateLimit = 500)
        {
            _cache = cache;
            _logger = logger;
            _ttlLength = ttlLength;
            _rateLimit = rateLimit;
        }

        public void CheckRateLimitForIpAddress(string ipAddress)
        {
            try
            {
                DateTime currentDate = DateTime.UtcNow;
                _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Retrieving or creating rate for {IpAddress}", ipAddress);

                uint count = _cache.GetOrCreate<uint>(ipAddress, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_ttlLength);
                    return 0;
                });

                _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Rate Count: {Count}, IP Address: {IpAddress}", count, ipAddress);

                if (HasRateLimitBeenReached(count))
                {
                    throw new TooManyRequestsException($"Rate limit of {_rateLimit} has been reached.");
                }

                 count++; 
                _cache.Set(ipAddress, count, TimeSpan.FromMinutes(_ttlLength));
            }
            catch (TooManyRequestsException ex)
            {
                throw new TooManyRequestsException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new AppException($"An error occured when checking rate limit - Error: {ex.Message}");
            }
        }

        private bool HasRateLimitBeenReached(uint count)
        {
            return count >= _rateLimit;
        }
    }
}
