using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Services;
using HabitTracker.Server.UnitsOfWork;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Server.Tests.Services
{
    public class HabitLogServiceTests
    {
        private readonly Mock<IHabitLogRepository> _mockHabitLogRepository;
        private readonly Mock<ITransactionalUnit<AddedHabitLogResult?, AddHabitLogData>> _mockAddHabitLogUnit;
        private readonly Mock<ILogger<HabitLogService>> _mockLogger;
        private readonly HabitLogService _service;

        public HabitLogServiceTests()
        {
            _mockHabitLogRepository = new Mock<IHabitLogRepository>();
            _mockAddHabitLogUnit = new Mock<ITransactionalUnit<AddedHabitLogResult?, AddHabitLogData>>();
            _mockLogger = new Mock<ILogger<HabitLogService>>();
            _service = new HabitLogService(_mockHabitLogRepository.Object, _mockLogger.Object, _mockAddHabitLogUnit.Object);
        }

        [Fact]
        public void GetById_ReturnsHabitLog()
        {
            int habitLogId = 1;
            int userId = 2;
            DateTime date = DateTime.UtcNow;

            _mockHabitLogRepository.Setup(repository => repository.GetById(habitLogId, userId)).Returns(new HabitLog(1, 2, date, true, 7));

            var result = _service.GetById(habitLogId, userId);

            Assert.NotNull(result);
            Assert.True(result.Id == 1);
            Assert.True(result.Habit_id == 2);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged == true);
            Assert.True(result.LengthInDays == 7);
        }

        [Fact]
        public void GetById_ThrowsNotFoundException()
        {
            int habitLogId = 1;
            int userId = 2;
            DateTime date = DateTime.UtcNow;

            _mockHabitLogRepository.Setup(repository => repository.GetById(habitLogId, userId)).Returns((HabitLog)null);

            Assert.Throws<NotFoundException>(() => _service.GetById(habitLogId, userId));
        }

        [Fact]
        public void GetById_ThrowsAppException()
        {
            int habitLogId = 1;
            int userId = 2;
            DateTime date = DateTime.UtcNow;

            _mockHabitLogRepository.Setup(repository => repository.GetById(habitLogId, userId)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.GetById(habitLogId, userId));
        }

        [Fact]
        public void GetAllByHabitId_ReturnsCollectionOfHabitLogs()
        {
            int habitId = 1;
            int userId = 2;
            uint pageNumber = 3;
            DateTime date = DateTime.UtcNow;

            _mockHabitLogRepository.Setup(repository => repository.GetAllByHabitId(habitId, userId, pageNumber, 30)).Returns(new PaginatedHabitLogs(new List<HabitLog> { new HabitLog(1, 2, date, true, 7) }, true));

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
        public void GetAllByHabitId_ThrowsNotFoundException()
        {
            int habitId = 1;
            int userId = 2;
            uint pageNumber = 3;
            DateTime date = DateTime.UtcNow;

            _mockHabitLogRepository.Setup(repository => repository.GetAllByHabitId(habitId, userId, pageNumber, 30)).Returns(new PaginatedHabitLogs(Array.Empty<HabitLog>(), false));

            Assert.Throws<NotFoundException>(() => _service.GetAllByHabitId(habitId, userId, pageNumber));
        }

        [Fact]
        public void GetAllByHabitId_ThrowsAppException()
        {
            int habitId = 1;
            int userId = 2;
            uint pageNumber = 3;
            DateTime date = DateTime.UtcNow;

            _mockHabitLogRepository.Setup(repository => repository.GetAllByHabitId(habitId, userId, pageNumber, 2)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.GetAllByHabitId(habitId, userId, pageNumber));
        }

        [Fact]
        public void Add_ReturnsCollectionOfHabitLogs()
        {
            DateTime date = DateTime.UtcNow;
            PostHabitLog habitLog = new PostHabitLog(1, date, true, 7);
            int userId = 1;
            var log = new HabitLog(1, 2, date, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);

            _mockAddHabitLogUnit.Setup(repository => repository.Execute(It.IsAny<AddHabitLogData>())).Returns(new AddedHabitLogResult(
                log,
                habit
            ));

            var result = _service.Add(habitLog, userId);

            Assert.NotNull(result);
            Assert.True(result.Habit == habit);
            Assert.True(result.HabitLog == log);
        }

        [Fact]
        public void Add_ThrowsConflictException()
        {
            DateTime date = DateTime.UtcNow;
            PostHabitLog habitLog = new PostHabitLog(1, date, true, 7);
            int userId = 1;
            var log = new HabitLog(1, 2, date, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);

            _mockAddHabitLogUnit.Setup(repository => repository.Execute(It.IsAny<AddHabitLogData>())).Throws<ConflictException>(() => throw new ConflictException("test"));

            Assert.Throws<ConflictException>(() => _service.Add(habitLog, userId));
        }

        [Fact]
        public void Add_ThrowsNotFoundException()
        {
            DateTime date = DateTime.UtcNow;
            PostHabitLog habitLog = new PostHabitLog(1, date, true, 7);
            int userId = 1;
            var log = new HabitLog(1, 2, date, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);

            _mockAddHabitLogUnit.Setup(repository => repository.Execute(It.IsAny<AddHabitLogData>())).Throws<NotFoundException>(() => throw new NotFoundException("test"));

            Assert.Throws<NotFoundException>(() => _service.Add(habitLog, userId));
        }

        [Fact]
        public void Add_ThrowsAppExceptionWhenAppExceptionIsThrown()
        {
            DateTime date = DateTime.UtcNow;
            PostHabitLog habitLog = new PostHabitLog(1, date, true, 7);
            int userId = 1;
            var log = new HabitLog(1, 2, date, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);

            _mockAddHabitLogUnit.Setup(repository => repository.Execute(It.IsAny<AddHabitLogData>())).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.Add(habitLog, userId));
        }

        [Fact]
        public void Add_ThrowsException()
        {
            DateTime date = DateTime.UtcNow;
            PostHabitLog habitLog = new PostHabitLog(1, date, true, 7);
            int userId = 1;
            var log = new HabitLog(1, 2, date, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);

            _mockAddHabitLogUnit.Setup(repository => repository.Execute(It.IsAny<AddHabitLogData>())).Throws<Exception>(() => throw new Exception("test"));

            Assert.Throws<AppException>(() => _service.Add(habitLog, userId));
        }

        [Fact]
        public void Update_ReturnsHabitLog()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);

            DateTime date = DateTime.UtcNow;
            _mockHabitLogRepository.Setup(repository => repository.Update(habitLog)).Returns(new HabitLog(1234, 4321, date, true, 1));

            var result = _service.Update(habitLog);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.Habit_id == 4321);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged);
            Assert.True(result.LengthInDays == 1);
        }

        [Fact]
        public void Update_ThrowsAppExceptionWhenNullIsReturned()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);

            _mockHabitLogRepository.Setup(repository => repository.Update(habitLog)).Returns((HabitLog)null);

            Assert.Throws<AppException>(() => _service.Update(habitLog));
        }

        [Fact]
        public void Update_ThrowsAppException()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);

            _mockHabitLogRepository.Setup(repository => repository.Update(habitLog)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.Update(habitLog));
        }

        [Fact]
        public void Delete_ReturnsTrue()
        {
            int habitLogId = 1;
            int userId = 2;
            HabitLog log = new HabitLog(1, 3, DateTime.UtcNow, true, 1);

            _mockHabitLogRepository.Setup(repository => repository.GetById(habitLogId, userId)).Returns(log);
            _mockHabitLogRepository.Setup(repository => repository.Delete(habitLogId, userId)).Returns(true);

            var result = _service.Delete(habitLogId, userId);

            Assert.True(result.Success);
            Assert.Equal(result.HabitLog, log);
        }

        [Fact]
        public void Delete_ReturnsFalseIfLogIsNotFound()
        {
            int habitLogId = 1;
            int userId = 2;

            _mockHabitLogRepository.Setup(repository => repository.GetById(habitLogId, userId)).Returns((HabitLog)null);
            var result = _service.Delete(habitLogId, userId);

            Assert.False(result.Success);
            Assert.Null(result.HabitLog);
        }
        
        [Fact]
        public void Delete_ReturnsFalseIfDeleteFails()
        {
            int habitLogId = 1;
            int userId = 2;
            HabitLog log = new HabitLog(1, 3, DateTime.UtcNow, true, 1);

            _mockHabitLogRepository.Setup(repository => repository.GetById(habitLogId, userId)).Returns(log);
            _mockHabitLogRepository.Setup(repository => repository.Delete(habitLogId, userId)).Returns(false);

            var result = _service.Delete(habitLogId, userId);

            Assert.False(result.Success);
            Assert.Null(result.HabitLog);
        }

        [Fact]
        public void Delete_ThrowsAppException()
        {
            int habitLogId = 1;
            int userId = 2;

            _mockHabitLogRepository.Setup(repository => repository.Delete(habitLogId, userId)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.Delete(habitLogId, userId));
        }
    }
}
