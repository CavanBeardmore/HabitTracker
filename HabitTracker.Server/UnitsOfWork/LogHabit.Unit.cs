using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.UnitsOfWork
{
    public class LogHabit : TransactionalUnit<Tuple<Habit, HabitLog>>
    {
        private readonly ILogger _logger;
        private readonly IHabitRepository _habitRepository;
        private readonly IHabitLogRepository _habitLogRepository;
        private readonly PostHabitLog _habitLog;
        private readonly int _userId;
        public LogHabit(ILogger logger, IHabitRepository habitRepository, IHabitLogRepository habitLogRepository, PostHabitLog habitLog, int userId) : base(habitRepository.SqliteFacade, logger) 
        {
            _logger = logger;
            _habitRepository = habitRepository;
            _habitLogRepository = habitLogRepository;
            _habitLog = habitLog;
            _userId = userId;
        }

        protected override Tuple<Habit, HabitLog> Work()
        {
            _logger.LogInformation("LogHabit - Work - checking if habit log exists");
            HabitLog? existingHabitLog = _habitLogRepository.GetByHabitIdAndStartDate(_habitLog.Habit_id, _userId, _habitLog.Start_date);

            if (existingHabitLog != null)
            {
                _logger.LogInformation("LogHabit - Work - habit log exists");
                throw new ConflictException("Habit log already exists for this date");
            }

            _logger.LogInformation("LogHabit - Work - getting habit");
            Habit? habit = _habitRepository.GetById(_habitLog.Habit_id, _userId, Transaction.Connection, Transaction.Transaction);

            if (habit == null)
            {
                _logger.LogInformation("LogHabit - Work - could not find habit");
                throw new NotFoundException($"Could not find Habit of {_habitLog.Habit_id} from Habit Log");
            }

            _logger.LogInformation("LogHabit - Work - current habit streak {@StreakCount}", habit.StreakCount);

            uint updatedStreakCount;

            if (habit.StreakCount == 0)
            {
                updatedStreakCount = 1;
            } 
            else
            {
                uint current = habit.StreakCount;
                updatedStreakCount = current + 1;
            }

            _logger.LogInformation("LogHabit - Work - updated habit streak {@UpdatedStreakCount}", updatedStreakCount);

            _logger.LogInformation("LogHabit - Work - adding habit log");
            HabitLog? habitLog = _habitLogRepository.Add(_habitLog, Transaction.Connection, Transaction.Transaction);

            if (habitLog == null)
            {
                _logger.LogInformation("LogHabit - Work - failed to add habit log");
                throw new AppException("Failed to add habit log");
            }

            _logger.LogInformation("LogHabit - Work - updating habit");
            bool success = _habitRepository.Update(_userId, new PatchHabit(habit.Id, habit.Name, updatedStreakCount), Transaction.Connection, Transaction.Transaction);

            if (!success)
            {
                _logger.LogInformation("LogHabit - Work - failed to update habit");
                throw new AppException("Failed to updated habit");
            }

            Habit? updatedHabit = _habitRepository.GetById(habit.Id, _userId, Transaction.Connection, Transaction.Transaction);

            if (updatedHabit == null)
            {
                _logger.LogInformation("LogHabit - Work - failed to get updated habit");
                throw new AppException("Failed to get updated habit");
            }

            _logger.LogInformation("LogHabit - Work - successfully completed LogHabit unit");
            return Tuple.Create(updatedHabit, habitLog);
        }
    }
}
