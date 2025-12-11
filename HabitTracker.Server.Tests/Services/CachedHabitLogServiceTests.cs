using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Models;
using HabitTracker.Server.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using System;

namespace HabitTracker.Server.Tests.Services
{
    public class CachedHabitLogServiceTests
    {
        private readonly Mock<IHabitLogService> _mockHabitLogService;
        private readonly Mock<ILogger<CachedHabitLogService>> _mockLogger;
        private readonly CachedHabitLogService _service;

        public CachedHabitLogServiceTests()
        {
            _mockHabitLogService = new Mock<IHabitLogService>();
            _mockLogger = new Mock<ILogger<CachedHabitLogService>>();
            _service = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, new MemoryCache(new MemoryCacheOptions()));
        }

        [Fact]
        public void GetById_ReturnsHabitLog()
        {
            int habitLogId = 1;
            int userId = 2;
            DateTime date = DateTime.UtcNow;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var serviceWithCache = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);
            var habitLog = new HabitLog(1, 2, date, true, 7);

            _mockHabitLogService.Setup(service => service.GetById(habitLogId, userId)).Returns(habitLog);

            var result = serviceWithCache.GetById(habitLogId, userId);

            Assert.NotNull(result);
            Assert.True(result.Id == 1);
            Assert.True(result.Habit_id == 2);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged == true);
            Assert.True(result.LengthInDays == 7);
            Assert.NotNull(cache.Get($"HabitLog_User_{userId}_HabitLog_{habitLogId}"));
            Assert.Equal(cache.Get($"HabitLog_User_{userId}_HabitLog_{habitLogId}"), habitLog);
        }

        [Fact]
        public void GetById_ReturnsCachedHabitLog()
        {
            int habitLogId = 1;
            int userId = 2;
            DateTime date = DateTime.UtcNow;

            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set($"HabitLog_User_{userId}_HabitLog_{habitLogId}", new HabitLog(1, 2, date, true, 7));
            var serviceWithCache = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);

            var result = serviceWithCache.GetById(habitLogId, userId);

            Assert.NotNull(result);
            Assert.True(result.Id == 1);
            Assert.True(result.Habit_id == 2);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged == true);
            Assert.True(result.LengthInDays == 7);
        }

        [Fact]
        public void GetAllByHabitId_ReturnsCollectionOfCachedHabitLogs()
        {
            int habitId = 1;
            int userId = 2;
            uint pageNumber = 3;
            DateTime date = DateTime.UtcNow;
            var cache = new MemoryCache(new MemoryCacheOptions());
            string version = Guid.NewGuid().ToString();
            cache.Set($"HabitLogsVersion_User_{userId}_Habit_{habitId}", version);
            cache.Set($"Version_{version}_Page_{pageNumber}", new PaginatedHabitLogs(new List<HabitLog> { new HabitLog(1, 2, date, true, 7) }, true));
            var serviceWithCache = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);
            var result = serviceWithCache.GetAllByHabitId(habitId, userId, pageNumber);

            Assert.NotNull(result);
            Assert.True(result.HabitLogs.Any());
            Assert.Contains(result.HabitLogs, hl => hl.Id == 1
                && hl.Habit_id == 2
                && hl.Start_date == date
                && hl.Habit_logged == true
                && hl.LengthInDays == 7
            );
        }

        [Fact]
        public void GetAllByHabitId_ReturnsCollectionOfHabitLogsWhenThereAreNoCachedLogs()
        {
            int habitId = 1;
            int userId = 2;
            uint pageNumber = 3;
            DateTime date = DateTime.UtcNow;

            _mockHabitLogService.Setup(repository => repository.GetAllByHabitId(habitId, userId, pageNumber)).Returns(new PaginatedHabitLogs(new List<HabitLog> { new HabitLog(1, 2, date, true, 7) }, true));

            var result = _service.GetAllByHabitId(habitId, userId, pageNumber);

            Assert.NotNull(result);
            Assert.True(result.HabitLogs.Any());
            Assert.Contains(result.HabitLogs, hl => hl.Id == 1
                && hl.Habit_id == 2
                && hl.Start_date == date
                && hl.Habit_logged == true
                && hl.LengthInDays == 7
            );
        }

        [Fact]
        public void GetAllByHabitId_ReturnsCollectionOfHabitLogsWhenVersionKeyDoesntMatch()
        {
            int habitId = 1;
            int userId = 2;
            uint pageNumber = 3;
            DateTime date = DateTime.UtcNow;
            var cache = new MemoryCache(new MemoryCacheOptions());
            string version1 = Guid.NewGuid().ToString();
            string version2 = Guid.NewGuid().ToString();
            cache.Set($"HabitLogsVersion_User_{userId}_Habit_{habitId}", version1);
            cache.Set($"Version_{version2}_Page_{pageNumber}", new PaginatedHabitLogs(new List<HabitLog> { new HabitLog(1, 2, date, true, 7) }, true));

            _mockHabitLogService.Setup(repository => repository.GetAllByHabitId(habitId, userId, pageNumber)).Returns(new PaginatedHabitLogs(new List<HabitLog> { new HabitLog(1, 2, date, true, 7) }, true));

            var serviceWithCache = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);
            var result = serviceWithCache.GetAllByHabitId(habitId, userId, pageNumber);

            Assert.NotNull(result);
            Assert.True(result.HabitLogs.Any());
            Assert.Contains(result.HabitLogs, hl => hl.Id == 1
                && hl.Habit_id == 2
                && hl.Start_date == date
                && hl.Habit_logged == true
                && hl.LengthInDays == 7
            );
        }

        [Fact]
        public void Add_ReturnsCollectionOfHabitLogs()
        {
            DateTime date = DateTime.UtcNow;
            PostHabitLog habitLog = new PostHabitLog(1, date, true, 7);
            int userId = 1;
            var log = new HabitLog(1, 2, date, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);

            _mockHabitLogService.Setup(service => service.Add(habitLog, userId)).Returns(new AddedHabitLogResult(
                log,
                habit
            ));

            var result = _service.Add(habitLog, userId);

            Assert.NotNull(result);
            Assert.True(result.Habit == habit);
            Assert.True(result.HabitLog == log);
        }

        [Fact]
        public void Add_RemovesHabitLogsFromCache()
        {
            DateTime date = DateTime.UtcNow;
            PostHabitLog habitLog = new PostHabitLog(2, date, true, 7);
            int userId = 1;
            var log = new HabitLog(1, 2, date, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);

            string key1 = $"HabitLog_User_{userId}_HabitLog_{1}";
            string key2 = $"MostRecent_HabitLog_User_{userId}_Habit_{2}";
            string key3 = $"HabitLogsVersion_User_{userId}_Habit_{2}";

            cache.Set(key1, 1234);
            cache.Set(key2, 1234);
            cache.Set(key3, 1234);

            _mockHabitLogService.Setup(service => service.Add(habitLog, userId)).Returns(new AddedHabitLogResult(
                log,
                habit
            ));

            var result = service.Add(habitLog, userId);

            Assert.NotNull(result);
            Assert.True(result.Habit == habit);
            Assert.True(result.HabitLog == log);
            Assert.Null(cache.Get(key1));
            Assert.Null(cache.Get(key2));
            Assert.NotEqual(cache.Get(key3), 1234);
        }

        [Fact]
        public void Add_DoesntRemoveFromCacheWhenNullIsReturned()
        {
            DateTime date = DateTime.UtcNow;
            PostHabitLog habitLog = new PostHabitLog(1, date, true, 7);
            int userId = 1;
            var log = new HabitLog(1, 2, date, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);

            string key1 = $"HabitLog_User_{userId}_HabitLog_{1}";
            string key2 = $"MostRecent_HabitLog_User_{userId}_Habit_{2}";
            string key3 = $"HabitLogsVersion_User_{userId}_Habit_{2}";

            cache.Set(key1, 1234);
            cache.Set(key2, 1234);
            cache.Set(key3, 1234);

            _mockHabitLogService.Setup(service => service.Add(habitLog, userId)).Returns((AddedHabitLogResult)null);

            var result = service.Add(habitLog, userId);

            Assert.Null(result);
            Assert.NotNull(cache.Get(key1));
            Assert.NotNull(cache.Get(key2));
            Assert.NotNull(cache.Get(key3));
        }

        [Fact]
        public void Update_ReturnsHabitLogAndRemovesFromCache()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);
            int userId = 1;
            DateTime date = DateTime.UtcNow;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);

            string key1 = $"HabitLog_User_{userId}_HabitLog_{1234}";
            string key2 = $"MostRecent_HabitLog_User_{userId}_Habit_{4321}";
            string key3 = $"HabitLogsVersion_User_{userId}_Habit_{4321}";

            cache.Set(key1, 1234);
            cache.Set(key2, 1234);
            cache.Set(key3, 1234);

            _mockHabitLogService.Setup(service => service.Update(habitLog, userId)).Returns(new HabitLog(1234, 4321, date, true, 1));

            var result = service.Update(habitLog, userId);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.Habit_id == 4321);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged);
            Assert.True(result.LengthInDays == 1);
            Assert.Null(cache.Get(key1));
            Assert.Null(cache.Get(key2));
            Assert.NotEqual(cache.Get(key3), 1234);
        }

        [Fact]
        public void Update_DoesntRemoveFromCacheWhenNullIsReturned()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);
            int userId = 1;
            DateTime date = DateTime.UtcNow;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);

            string key1 = $"HabitLog_User_{userId}_HabitLog_{1234}";
            string key2 = $"MostRecent_HabitLog_User_{userId}_Habit_{4321}";
            string key3 = $"HabitLogsVersion_User_{userId}_Habit_{4321}";

            cache.Set(key1, 1234);
            cache.Set(key2, 1234);
            cache.Set(key3, 1234);

            _mockHabitLogService.Setup(service => service.Update(habitLog, userId)).Returns((HabitLog)null);

            var result = _service.Update(habitLog, userId);

            Assert.Null(result);
            Assert.NotNull(cache.Get(key1));
            Assert.NotNull(cache.Get(key2));
            Assert.NotNull(cache.Get(key3));
        }

        [Fact]
        public void Delete_ReturnsTrueAndRemovesFromCache()
        {
            int habitLogId = 1;
            int userId = 2;
            HabitLog log = new HabitLog(1, 3, DateTime.UtcNow, true, 1);

            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);

            string key1 = $"HabitLog_User_{userId}_HabitLog_{habitLogId}";
            string key2 = $"MostRecent_HabitLog_User_{userId}_Habit_{3}";
            string key3 = $"HabitLogsVersion_User_{userId}_Habit_{3}";

            cache.Set(key1, 1234);
            cache.Set(key2, 1234);
            cache.Set(key3, 1234);

            _mockHabitLogService.Setup(service => service.Delete(habitLogId, userId)).Returns(new DeleteHabitLogResult(true, log));

            var result = service.Delete(habitLogId, userId);

            Assert.True(result.Success);
            Assert.Equal(result.HabitLog, log);
            Assert.Null(cache.Get(key1));
            Assert.Null(cache.Get(key2));
            Assert.NotEqual(cache.Get(key3), 1234);
        }

        [Fact]
        public void Delete_ReturnsFalseIfLogIsNotFoundAndDoesntRemoveFromCache()
        {
            int habitLogId = 1;
            int userId = 2;

            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);

            string key1 = $"HabitLog_User_{userId}_HabitLog_{1234}";
            string key2 = $"MostRecent_HabitLog_User_{userId}_Habit_{4321}";
            string key3 = $"HabitLogsVersion_User_{userId}_Habit_{4321}";

            cache.Set(key1, 1234);
            cache.Set(key2, 1234);
            cache.Set(key3, 1234);

            _mockHabitLogService.Setup(service => service.Delete(habitLogId, userId)).Returns(new DeleteHabitLogResult(false, null));
            var result = _service.Delete(habitLogId, userId);

            Assert.False(result.Success);
            Assert.Null(result.HabitLog);
            Assert.NotNull(cache.Get(key1));
            Assert.NotNull(cache.Get(key2));
            Assert.NotNull(cache.Get(key3));
        }

        [Fact]
        public void Delete_ReturnsFalseIfDeleteFails()
        {
            int habitLogId = 1;
            int userId = 2;
            HabitLog log = new HabitLog(1, 3, DateTime.UtcNow, true, 1);


            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new CachedHabitLogService(_mockHabitLogService.Object, _mockLogger.Object, cache);

            string key1 = $"HabitLog_User_{userId}_HabitLog_{1234}";
            string key2 = $"MostRecent_HabitLog_User_{userId}_Habit_{4321}";
            string key3 = $"HabitLogsVersion_User_{userId}_Habit_{4321}";

            cache.Set(key1, 1234);
            cache.Set(key2, 1234);
            cache.Set(key3, 1234);

            _mockHabitLogService.Setup(service => service.Delete(habitLogId, userId)).Returns(new DeleteHabitLogResult(false, null));

            var result = _service.Delete(habitLogId, userId);

            Assert.False(result.Success);
            Assert.Null(result.HabitLog);
            Assert.NotNull(cache.Get(key1));
            Assert.NotNull(cache.Get(key2));
            Assert.NotNull(cache.Get(key3));
        }
    }
}
