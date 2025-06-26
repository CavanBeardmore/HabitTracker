﻿using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Database.Entities;

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

            _logger.LogInformation("LogHabit - Work - getting most recent habit log");
            HabitLog? mostRecentHabitLog = _habitLogRepository.GetMostRecentHabitLog(_habitLog.Habit_id, _userId, Transaction.Connection, Transaction.Transaction);

            _logger.LogInformation("LogHabit - Work - getting habit");
            Habit? habit = _habitRepository.GetById(_habitLog.Habit_id, _userId, Transaction.Connection, Transaction.Transaction);

            if (habit == null)
            {
                _logger.LogInformation("LogHabit - Work - could not find habit");
                throw new NotFoundException($"Could not find Habit of {_habitLog.Habit_id} from Habit Log");
            }

            _logger.LogInformation("LogHabit - Work - current habit streak {@StreakCount}", habit.StreakCount);

            uint updatedStreakCount = GetStreakCount(habit.StreakCount, mostRecentHabitLog?.Start_date);

            _logger.LogInformation("LogHabit - Work - updated habit streak {@UpdatedStreakCount}", updatedStreakCount);

            _logger.LogInformation("LogHabit - Work - adding habit log");
            HabitLog? habitLog = _habitLogRepository.Add(_habitLog, Transaction.Connection, Transaction.Transaction);

            if (habitLog == null)
            {
                _logger.LogInformation("LogHabit - Work - failed to add habit log");
                throw new AppException("Failed to add habit log");
            }

            _logger.LogInformation("LogHabit - Work - updating habit");
            Habit? updatedHabit = _habitRepository.Update(_userId, new PatchHabit(habit.Id, habit.Name, updatedStreakCount), Transaction.Connection, Transaction.Transaction);

            if (updatedHabit == null)
            {
                _logger.LogInformation("LogHabit - Work - failed to update habit");
                throw new AppException("Failed to updated habit");
            }

            _logger.LogInformation("LogHabit - Work - successfully completed LogHabit unit");
            return Tuple.Create(updatedHabit, habitLog);
        }

        private uint GetStreakCount(uint currentStreakCount, DateTime? mostRecentLoggedDate)
        {
            if (currentStreakCount == 0 && mostRecentLoggedDate == null)
            {
                return 1;
            }

            if (mostRecentLoggedDate == null)
            {
                _logger.LogInformation("LogHabit - Work - most recent logged date is null");
                throw new NotFoundException("Most recent logged date is null");
            }

            TimeSpan daysInBetween = _habitLog.Start_date.Date.Subtract(mostRecentLoggedDate.Value.Date);

            if (daysInBetween.Days == 1)
            {
                return currentStreakCount + 1;
            }

            return 1;
        }
    }
}
