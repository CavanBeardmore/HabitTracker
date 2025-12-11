using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HabitTracker.Server.Services
{
    public class CachedHabitService : IHabitService
    {
        private readonly TimeSpan _ttlInMinutes = TimeSpan.FromMinutes(30);

        private readonly IHabitService _innerService;
        private readonly ILogger<CachedHabitService> _logger;
        private readonly IMemoryCache _cache;
        
        public CachedHabitService(IHabitService innerService, ILogger<CachedHabitService> logger, IMemoryCache cache)
        {
            _innerService = innerService;
            _logger = logger;
            _cache = cache;
        }

        private string GenerateSingleHabitCacheKey(int userId, int habitId) => $"Habit_User_{userId}_Habit_{habitId}";

        private string GenerateMultiHabitCacheKey(int userId) => $"Habits_User_{userId}";

        public Habit? GetById(int habitId, int userId)
        {
            _logger.LogInformation("CachedHabitService - GetById - Fetching cached habit {HabitId} for user {UserId}", habitId, userId);

            Habit? cachedHabit = _cache.Get<Habit?>(GenerateSingleHabitCacheKey(userId, habitId));

            if (cachedHabit != null)
            {
                _logger.LogInformation("CachedHabitService - GetById - found cached habit");
                return cachedHabit;
            }

            Habit? habit = _innerService.GetById(habitId, userId);

            if (habit != null)
            {
                _logger.LogInformation("CachedHabitService - GetById - adding habit to cache");
                _cache.Set(GenerateSingleHabitCacheKey(userId, habitId), habit, _ttlInMinutes);
            }

            return habit;
        }

        public IReadOnlyCollection<Habit?> GetAllByUserId(int userId)
        {
            _logger.LogInformation("CachedHabitService - GetAllByUserId - Fetching collection of cached habits for user {UserId}", userId);

            IReadOnlyCollection<Habit?>? cachedHabits = _cache.Get<IReadOnlyCollection<Habit?>>(GenerateMultiHabitCacheKey(userId));

            if (cachedHabits != null && cachedHabits.Any())
            {
                _logger.LogInformation("CachedHabitService - GetAllByUserId - found cached habits");
                return cachedHabits;
            }

            IReadOnlyCollection<Habit?> habits = _innerService.GetAllByUserId(userId);

            if (habits.Any())
            {
                _logger.LogInformation("CachedHabitService - GetAllByUserId - adding collection of habits to cache");
                _cache.Set(GenerateMultiHabitCacheKey(userId), habits, _ttlInMinutes);
            }

            return habits;
        }

        public Habit? Add(int userId, PostHabit habit)
        {
            Habit? createdHabit = _innerService.Add(userId, habit);

            if (createdHabit != null)
            {
                _logger.LogInformation("CachedHabitService - Add - adding newly created habit to cache");
                _cache.Set(GenerateSingleHabitCacheKey(userId, createdHabit.Id), createdHabit, _ttlInMinutes);
                _cache.Remove(GenerateMultiHabitCacheKey(userId));
            }

            return createdHabit;
        }

        public Habit? Update(int userId, PatchHabit habit)
        {
            Habit? updatedHabit = _innerService.Update(userId, habit);

            if (updatedHabit != null)
            {
                _logger.LogInformation("CachedHabitService - Update - updating cached habit");
                _cache.Set(GenerateSingleHabitCacheKey(userId, updatedHabit.Id), updatedHabit, _ttlInMinutes);
                _cache.Remove(GenerateMultiHabitCacheKey(userId));
            }

            return updatedHabit;
        }

        public bool Delete(int habitId, int userid)
        {
            bool deleteResult = _innerService.Delete(habitId, userid);

            if (deleteResult)
            {
                _logger.LogInformation("CachedHabitService - Delete - removing deleted habit from cache");
                _cache.Remove(GenerateSingleHabitCacheKey(userid, habitId));
                _cache.Remove(GenerateMultiHabitCacheKey(userid));
            }

            return deleteResult;
        }
    }
}
