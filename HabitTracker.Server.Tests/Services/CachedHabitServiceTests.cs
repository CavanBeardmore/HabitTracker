using HabitTracker.Server.Database.Entities;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Server.Tests.Services
{
    public class CachedHabitServiceTests
    {
        private readonly Mock<IHabitService> _mockHabitService;
        private readonly Mock<ILogger<CachedHabitService>> _mockLogger;
        private readonly CachedHabitService _service;

        public CachedHabitServiceTests()
        {
            _mockHabitService = new Mock<IHabitService>();
            _mockLogger = new Mock<ILogger<CachedHabitService>>();
            _service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, new MemoryCache(new MemoryCacheOptions()));
        }

        [Fact]
        public void GetById_ReturnsHabitFromCache()
        {
            int habitId = 1;
            int userId = 2;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var cachedHabit = new Habit(habitId, userId, "test", 7);

            cache.Set($"Habit_User_{userId}_Habit_{habitId}", cachedHabit);

            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.GetById(habitId, userId);

            Assert.Equal(cachedHabit, result);
        }

        [Fact]
        public void GetById_ReturnsHabitFromServiceWhenNotCached()
        {
            int habitId = 1;
            int userId = 2;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var habit = new Habit(habitId, userId, "test", 7);

            _mockHabitService.Setup(s => s.GetById(habitId, userId)).Returns(habit);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.GetById(habitId, userId);

            Assert.Equal(habit, result);
        }

        [Fact]
        public void GetById_AddsHabitFromServiceToCache()
        {
            int habitId = 1;
            int userId = 2;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var habit = new Habit(habitId, userId, "test", 7);

            _mockHabitService.Setup(s => s.GetById(habitId, userId)).Returns(habit);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.GetById(habitId, userId);

            Assert.Equal(cache.Get($"Habit_User_{userId}_Habit_{habitId}"), habit);
            Assert.Equal(habit, result);
        }

        [Fact]
        public void GetAllByUserId_ReturnsHabitsFromCache()
        {
            int habitId = 1;
            int userId = 2;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var cachedHabits = new List<Habit>{ new Habit(habitId, userId, "test", 7) };

            cache.Set($"Habits_User_{userId}", cachedHabits);

            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.GetAllByUserId(userId);

            Assert.Equal(cachedHabits, result);
        }

        [Fact]
        public void GetAllByUserId_ReturnsHabitsFromServiceWhenNotCached()
        {
            int habitId = 1;
            int userId = 2;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var habits = new List<Habit> { new Habit(habitId, userId, "test", 7) };

            _mockHabitService.Setup(s => s.GetAllByUserId(userId)).Returns(habits);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.GetAllByUserId(userId);

            Assert.Equal(habits, result);
        }

        [Fact]
        public void GetAllByUserId_AddsHabitFromServiceToCache()
        {
            int habitId = 1;
            int userId = 2;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var habits = new List<Habit> { new Habit(habitId, userId, "test", 7) };

            _mockHabitService.Setup(s => s.GetAllByUserId(userId)).Returns(habits);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.GetAllByUserId(userId);

            Assert.Equal(cache.Get($"Habits_User_{userId}"), habits);
            Assert.Equal(habits, result);
        }

        [Fact]
        public void Add_AddsHabitFromServiceToCacheAndReturnsIt()
        {
            int userId = 1;
            PostHabit habit = new PostHabit("test");

            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set($"Habits_User_{userId}", new List<Habit> { new Habit(4, 1, "test", 7) });
            var serviceHabit = new Habit(2, userId, "test", 7);

            _mockHabitService.Setup(s => s.Add(userId, habit)).Returns(serviceHabit);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.Add(userId, habit);

            Assert.Equal(cache.Get($"Habit_User_{userId}_Habit_{2}"), serviceHabit);
            Assert.Null(cache.Get($"Habits_User_{userId}"));
            Assert.Equal(serviceHabit, result);
        }

        [Fact]
        public void Add_DoesntAddToCacheWhenServiceReturnsNull()
        {
            int userId = 1;
            PostHabit habit = new PostHabit("test");

            var cache = new MemoryCache(new MemoryCacheOptions());
            var serviceHabit = new Habit(2, userId, "test", 7);

            _mockHabitService.Setup(s => s.Add(userId, habit)).Returns((Habit)null);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.Add(userId, habit);

            Assert.Null(cache.Get($"Habit_User_{userId}_Habit_{2}"));
            Assert.Null(result);
        }

        [Fact]
        public void Update_AddsHabitFromServiceToCacheAndReturnsIt()
        {
            int userId = 1;
            PatchHabit habit = new PatchHabit(2, "test", 7);

            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set($"Habits_User_{userId}", new List<Habit> { new Habit(4, 1, "test", 7) });
            var serviceHabit = new Habit(2, userId, "test", 7);

            _mockHabitService.Setup(s => s.Update(userId, habit)).Returns(serviceHabit);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.Update(userId, habit);

            Assert.Equal(cache.Get($"Habit_User_{userId}_Habit_{2}"), serviceHabit);
            Assert.Null(cache.Get($"Habits_User_{userId}"));
            Assert.Equal(serviceHabit, result);
        }

        [Fact]
        public void Update_DoesntAddToCacheWhenServiceReturnsNull()
        {
            int userId = 1;
            PatchHabit habit = new PatchHabit(2, "test", 7);

            var cache = new MemoryCache(new MemoryCacheOptions());
            var serviceHabit = new Habit(2, userId, "test", 7);

            _mockHabitService.Setup(s => s.Update(userId, habit)).Returns((Habit)null);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.Update(userId, habit);

            Assert.Null(cache.Get($"Habit_User_{userId}_Habit_{2}"));
            Assert.Null(result);
        }

        [Fact]
        public void Delete_RemovesFromCacheWhenDeleteIsSuccessful()
        {
            int userId = 1;
            int habitId = 4;

            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set($"Habits_User_{userId}_Habit_{habitId}", new Habit(habitId, 1, "test", 7));
            cache.Set($"Habits_User_{userId}", new List<Habit> { new Habit(habitId, 1, "test", 7) });

            _mockHabitService.Setup(s => s.Delete(habitId, userId)).Returns(true);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.Delete(habitId, userId);

            Assert.Null(cache.Get($"Habit_User_{userId}_Habit_{habitId}"));
            Assert.Null(cache.Get($"Habit_User_{userId}"));
            Assert.True(result);
        }

        [Fact]
        public void Delete_DoesntRemoveFromCacheWhenDeleteFails()
        {
            int userId = 1;
            int habitId = 4;
            Habit habit = new Habit(habitId, 1, "test", 7);
            List<Habit> habits = new List<Habit> { new Habit(habitId, 1, "test", 7) };
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set($"Habit_User_{userId}_Habit_{habitId}", habit);
            cache.Set($"Habits_User_{userId}", habits);

            _mockHabitService.Setup(s => s.Delete(habitId, userId)).Returns(false);
            var service = new CachedHabitService(_mockHabitService.Object, _mockLogger.Object, cache);

            var result = service.Delete(habitId, userId);

            Assert.Equal(cache.Get($"Habit_User_{userId}_Habit_{habitId}"), habit);
            Assert.Equal(cache.Get($"Habits_User_{userId}"), habits);
            Assert.False(result);
        }
    }
}
