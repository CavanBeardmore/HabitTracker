using HabitTracker.Server.DTOs;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Exceptions;
using System.Net;
using Xunit.Abstractions;

namespace HabitTracker.Server.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly uint _ttlLength;
        private readonly uint _rateLimit;
        private readonly IRateLimitRepository _rateLimitRepository;
        private readonly ILogger<RateLimitService> _logger;

        public RateLimitService(IRateLimitRepository rateLimitRepository, ILogger<RateLimitService> logger, uint ttlLength = 5, uint rateLimit = 100)
        {
            _rateLimitRepository = rateLimitRepository;
            _logger = logger;
            _ttlLength = ttlLength;
            _rateLimit = rateLimit;
        }

        public bool HasIpAddressBeenLimited(string ipAddress)
        {
            DateTime currentDate = DateTime.UtcNow;
            Rate? existingRate = _rateLimitRepository.GetRate(ipAddress);

            if (existingRate == null)
            {
                return false;
            }

            return (HasRateExpired(existingRate, currentDate) == false && HasRateLimitBeenReached(existingRate) == true);
        }

        public void CheckRateLimitForIpAddress(string ipAddress)
        {
            try
            {
                DateTime currentDate = DateTime.UtcNow;
                _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Retrieving existing rate for {IpAddress}", ipAddress);

                Rate? existingRate = _rateLimitRepository.GetRate(ipAddress);

                if (existingRate == null)
                {
                    _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - No rate exists for {IpAddress}", ipAddress);
                    HandleCreateRate(ipAddress, currentDate);
                    return;
                }

                if (HasRateExpired(existingRate, currentDate))
                {
                    HandleExpiredRate(existingRate, currentDate);
                    return;
                }

                if (HasRateLimitBeenReached(existingRate))
                {
                    HandleStartLimitation(existingRate);
                    throw new TooManyRequestsException($"Rate limit of {_rateLimit} has been reached.");
                }

                HandleIncrementRateCount(existingRate);

            }
            catch (TooManyRequestsException ex)
            {
                throw new TooManyRequestsException(ex.Message);
            }
            catch (AppException ex)
            {
                throw new AppException(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new AppException("An error occured when checking rate limit");
            }
        }

        private bool HasRateExpired(Rate existingRate, DateTime currentDate)
        {
            return existingRate.Ttl.CompareTo(currentDate) < 0;
        }

        private void HandleExpiredRate(Rate existingRate, DateTime currentDate)
        {
            uint count = 1;
            DateTime newDate = currentDate.AddMinutes(_ttlLength);
            _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Handling expired rate for {IpAddress}", existingRate.IpAddress);
            bool success = _rateLimitRepository.UpdateRateCountAndTtl(count, existingRate.IpAddress, newDate);

            if (success == false)
            {
                _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Failed to update expired rate for {IpAddress} - count {Count} - TTL {Ttl}", existingRate.IpAddress, count, newDate);
                throw new AppException($"Failed to update expired rate");
            }
        }

        private void HandleStartLimitation(Rate existingRate)
        {
            _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Rate limit reach starting timeout for {IpAddress}", existingRate.IpAddress);
            DateTime timeoutDate = existingRate.Ttl.AddMinutes(_ttlLength);
            bool success = _rateLimitRepository.UpdateRateCountAndTtl(existingRate.Count, existingRate.IpAddress, timeoutDate);

            if (success == false)
            {
                _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Unable to start timeout session for {IpAddress}", existingRate.IpAddress);
                throw new AppException("Failed to update rate count and ttl");
            }
        }

        private bool HasRateLimitBeenReached(Rate existingRate)
        {
            return existingRate.Count >= _rateLimit;
        }

        private void HandleCreateRate(string ipAddress, DateTime date) 
        {
            _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Creating rate for {IpAddress}", ipAddress);
            Rate? createdRate = _rateLimitRepository.AddRate(1, ipAddress, date.AddMinutes(_ttlLength));

            if (createdRate == null)
            {
                _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Failed to create rate for {IpAddress}", ipAddress);
                throw new AppException($"Failed to create rate for {ipAddress}");
            }

            _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Created rate for {IpAddress}", ipAddress);
        }

        private void HandleIncrementRateCount(Rate existingRate)
        {
            uint count = existingRate.Count + 1;
            _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Handling incrementing rate for {IpAddress} to {Count}", existingRate.IpAddress, count);

            bool success = _rateLimitRepository.UpdateRateCount(count, existingRate.IpAddress);

            if (success == false)
            {
                _logger.LogInformation("RateLimitService - CheckRateLimitForIpAddress - Failed to update rate count for {IpAddress}", existingRate.IpAddress);
                throw new AppException("Failed to update rate count");
            }
        }
    }
}
