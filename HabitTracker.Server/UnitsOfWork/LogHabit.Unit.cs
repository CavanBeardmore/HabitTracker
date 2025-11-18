using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.UnitsOfWork
{
    public class LogHabit : TransactionalUnit<AddedHabitLogResult?, AddHabitLogData>
    {
        private readonly ILogger _logger;
        private readonly IHabitRepository _habitRepository;
        private readonly IHabitLogRepository _habitLogRepository;
        public LogHabit(ILogger logger, IHabitRepository habitRepository, IHabitLogRepository habitLogRepository) : base(habitRepository.SqliteFacade, logger) 
        {
            _logger = logger;
            _habitRepository = habitRepository;
            _habitLogRepository = habitLogRepository;
        }

        protected override AddedHabitLogResult Work(AddHabitLogData args)
        {
            PostHabitLog postHabitLog = args.Habit;
            int userId = args.UserId;

            _logger.LogInformation("LogHabit - Work - checking if habit log exists");
            HabitLog? existingHabitLog = _habitLogRepository.GetByHabitIdAndStartDate(postHabitLog.Habit_id, userId, postHabitLog.Start_date);

            if (existingHabitLog != null)
            {
                _logger.LogInformation("LogHabit - Work - habit log exists");
                throw new ConflictException("Habit log already exists for this date");
            }

            _logger.LogInformation("LogHabit - Work - getting most recent habit log");
            HabitLog? mostRecentHabitLog = _habitLogRepository.GetMostRecentHabitLog(postHabitLog.Habit_id, userId, Transaction.Connection, Transaction.Transaction);

            _logger.LogInformation("LogHabit - Work - got most recent habit log {@Log}", mostRecentHabitLog);

            _logger.LogInformation("LogHabit - Work - getting habit");
            Habit? habit = _habitRepository.GetById(postHabitLog.Habit_id, userId, Transaction.Connection, Transaction.Transaction);

            if (habit == null)
            {
                _logger.LogInformation("LogHabit - Work - could not find habit");
                throw new NotFoundException($"Could not find Habit of {postHabitLog.Habit_id} from Habit Log");
            }

            _logger.LogInformation("LogHabit - Work - current habit streak {@StreakCount}", habit.StreakCount);

            uint updatedStreakCount = GetStreakCount(habit.StreakCount, mostRecentHabitLog?.Start_date, postHabitLog.Start_date);

            _logger.LogInformation("LogHabit - Work - updated habit streak {@UpdatedStreakCount}", updatedStreakCount);

            _logger.LogInformation("LogHabit - Work - adding habit log");
            HabitLog? habitLog = _habitLogRepository.Add(postHabitLog, Transaction.Connection, Transaction.Transaction);

            if (habitLog == null)
            {
                _logger.LogInformation("LogHabit - Work - failed to add habit log");
                throw new AppException("Failed to add habit log");
            }

            _logger.LogInformation("LogHabit - Work - updating habit");
            Habit? updatedHabit = _habitRepository.Update(userId, new PatchHabit(habit.Id, habit.Name, updatedStreakCount), Transaction.Connection, Transaction.Transaction);

            if (updatedHabit == null)
            {
                _logger.LogInformation("LogHabit - Work - failed to update habit");
                throw new AppException("Failed to updated habit");
            }

            _logger.LogInformation("LogHabit - Work - successfully completed LogHabit unit");
            return new AddedHabitLogResult(habitLog, updatedHabit);
        }

        private uint GetStreakCount(uint currentStreakCount, DateTime? mostRecentLoggedDate, DateTime newHabitLogStartDate)
        {
            if (currentStreakCount == 0 && mostRecentLoggedDate == null)
            {
                _logger.LogInformation("LogHabit - GetStreakCount - Starting off a new streak");
                return 1;
            }

            if (mostRecentLoggedDate == null)
            {
                _logger.LogInformation("LogHabit - Work - most recent logged date is null");
                throw new NotFoundException("Most recent logged date is null");
            }

            _logger.LogInformation("LogHabit - GetStreakCount - previous log date: {Previous}, new log date: {New}", mostRecentLoggedDate.Value.Date, newHabitLogStartDate);

            TimeSpan daysInBetween = newHabitLogStartDate.Date.Subtract(mostRecentLoggedDate.Value.Date);
            _logger.LogInformation("LogHabit - GetStreakCount - days in between - {daysInBetween}", daysInBetween.Days);
            if (daysInBetween.Days == 0)
            {
                _logger.LogInformation("LogHabit - GetStreakCount - incrementing streak count");
                return currentStreakCount + 1;
            }

            _logger.LogInformation("LogHabit - GetStreakCount - defaulting new streak count to 1");
            return 1;
        }
    }
}
