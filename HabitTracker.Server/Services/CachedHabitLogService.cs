using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace HabitTracker.Server.Services
{
    public class CachedHabitLogService : IHabitLogService
    {
        private readonly TimeSpan _ttlInMinutes = TimeSpan.FromMinutes(30);

        private readonly IHabitLogService _innerService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public CachedHabitLogService(
            IHabitLogService innerService,
            ILogger<CachedHabitLogService> logger,
            IMemoryCache memoryCache
        )
        {
            _innerService = innerService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        private string GenerateSingleLogCacheKey(int userId, int habitLogId) => $"HabitLog_User_{userId}_HabitLog_{habitLogId}";

        private string GenerateMostRecentLogCacheKey(int userId, int habitId) => $"MostRecent_HabitLog_User_{userId}_Habit_{habitId}";

        private string GenerateMultiLogsVersionKey(int userId, int habitId) => $"HabitLogsVersion_User_{userId}_Habit_{habitId}";

        private string GenerateMultiLogsCacheKey(string version, uint pageNumber) => $"Version_{version}_Page_{pageNumber}";

        public PaginatedHabitLogs? GetAllByHabitId(int id, int userId, uint pageNumber)
        {
            _logger.LogInformation("CachedHabitLogService - GetAllByHabitId - getting all cached habit logs from habit of {id} for {userId} with page no. {pageNumber}", id, userId, pageNumber);
            string? versionKey = _memoryCache.Get<string>(GenerateMultiLogsVersionKey(userId, id));

            if (versionKey != null)
            {
                _logger.LogInformation("CachedHabitLogService - GetAllByHabitId - getting all cachedhabit logs using version key {versionKey}", versionKey);
                PaginatedHabitLogs? cachedHabitLogs = _memoryCache.Get<PaginatedHabitLogs>(GenerateMultiLogsCacheKey(versionKey, pageNumber));

                if (cachedHabitLogs != null)
                {
                    _logger.LogInformation("CachedHabitLogService - GetAllByHabitId - found cached habit logs");
                    return cachedHabitLogs;
                }
            }

            PaginatedHabitLogs? habitLogs = _innerService.GetAllByHabitId(id, userId, pageNumber);

            if (habitLogs != null) 
            {
                _logger.LogInformation("CachedHabitLogService - GetAllByHabitId - caching paginated habit logs");
                string newVersionKey = Guid.NewGuid().ToString();
                _memoryCache.Set(GenerateMultiLogsVersionKey(userId, id), newVersionKey, _ttlInMinutes);
                _memoryCache.Set(GenerateMultiLogsCacheKey(newVersionKey, pageNumber), habitLogs, _ttlInMinutes);
            }

            return habitLogs;
        }

        public HabitLog? GetById(int habitLogId, int userId)
        {
            _logger.LogInformation("CachedHabitLogService - GetById - getting cached habit using habit log id: {habitLogId} and user id: {userId}", habitLogId, userId);
            HabitLog? cachedHabitLog = _memoryCache.Get<HabitLog>(GenerateSingleLogCacheKey(userId, habitLogId));

            if (cachedHabitLog != null)
            {
                _logger.LogInformation("CachedHabitLogService - GetById - found cached habit log");
                return cachedHabitLog;
            }

            HabitLog? habitLog = _innerService.GetById(habitLogId, userId);

            if (habitLog != null) 
            {
                _logger.LogInformation("CachedHabitLogService - GetById - caching habit log");
                _memoryCache.Set(GenerateSingleLogCacheKey(userId, habitLogId), habitLog, _ttlInMinutes);
            }

            return habitLog;
        }

        public HabitLog? GetMostRecentByHabitId(int habitId, int userId)
        {
            _logger.LogInformation("CachedHabitLogService - GetMostRecentByHabitId - getting most recent habit logs using habit id {habitId} and user id {userId}", habitId, userId);
            HabitLog? cachedHabitLog = _memoryCache.Get<HabitLog>(GenerateMostRecentLogCacheKey(userId, habitId));

            if (cachedHabitLog != null)
            {
                _logger.LogInformation("CachedHabitLogService - GetMostRecentByHabitId - found cached most recent habit log");
                return cachedHabitLog; 
            }

            HabitLog? habitLog = _innerService.GetMostRecentByHabitId(habitId, userId);

            if (habitLog != null)
            {
                _logger.LogInformation("CachedHabitLogService - GetMostRecentByHabitId - caching most recent habit log");
                _memoryCache.Set(GenerateMostRecentLogCacheKey(userId, habitId), habitLog, _ttlInMinutes);
            }

            return habitLog;
        }

        public AddedHabitLogResult? Add(PostHabitLog habitLog, int userId)
        {
            AddedHabitLogResult? addedResult = _innerService.Add(habitLog, userId);

            if (addedResult != null)
            {
                _logger.LogInformation("CachedHabitLogService - Add - removing habit logs from cache");
                _memoryCache.Set(GenerateMultiLogsVersionKey(userId, habitLog.Habit_id), Guid.NewGuid().ToString(), _ttlInMinutes);
                _memoryCache.Remove(GenerateMostRecentLogCacheKey(userId, habitLog.Habit_id));
                _memoryCache.Remove(GenerateSingleLogCacheKey(userId, addedResult.HabitLog.Id));
            }

            return addedResult;
        }

        public HabitLog? Update(PatchHabitLog habitLog, int userId)
        {
            HabitLog? updatedLog = _innerService.Update(habitLog, userId);

            if (updatedLog != null)
            {
                _logger.LogInformation("CachedHabitLogService - Update - removing habit logs from cache");
                _memoryCache.Set(GenerateMultiLogsVersionKey(userId, updatedLog.Habit_id), Guid.NewGuid().ToString(), _ttlInMinutes);
                _memoryCache.Remove(GenerateMostRecentLogCacheKey(userId, updatedLog.Habit_id));
                _memoryCache.Remove(GenerateSingleLogCacheKey(userId, updatedLog.Id));
            }

            return updatedLog;
        }

        public DeleteHabitLogResult Delete(int habitLogId, int userId)
        {
            DeleteHabitLogResult result = _innerService.Delete(habitLogId, userId);

            if (result.Success && result.HabitLog != null)
            {
                _logger.LogInformation("CachedHabitLogService - Delete - removing habit logs from cache");
                _memoryCache.Set(GenerateMultiLogsVersionKey(userId, result.HabitLog.Habit_id), Guid.NewGuid().ToString(), _ttlInMinutes);
                _memoryCache.Remove(GenerateMostRecentLogCacheKey(userId, result.HabitLog.Habit_id));
                _memoryCache.Remove(GenerateSingleLogCacheKey(userId, habitLogId));
            }

            return result;
        }
    }
}
