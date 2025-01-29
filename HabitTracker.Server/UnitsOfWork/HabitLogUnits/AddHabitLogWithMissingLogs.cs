using HabitTracker.Server.Database.Entities;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;

namespace HabitTracker.Server.UnitsOfWork.HabitLogUnits
{
    public class AddHabitLogWithMissingLogs
    {
        private List<DateTime> _successfullyAddedLogDates = new List<DateTime>();
        private readonly PostHabitLog _postHabitLog;
        private readonly IHabitLogRepository _repository;
        public AddHabitLogWithMissingLogs(PostHabitLog postHabitLog, IHabitLogRepository repository)
        {
            _postHabitLog = postHabitLog;
            _repository = repository;
        }

        public UnitOfWorkResult<bool> execute()
        {
            var habitlog = _repository.GetMostRecentHabitLog(_postHabitLog.Habit_id, _postHabitLog.User_id);

            int habitsToAdd = CalculateAmountOfHabitsToAdd(habitlog);

            for (int i = habitsToAdd; i > 0; i--)
            {
                DateTime date = _postHabitLog.Start_date.AddDays(-i);

                bool attempt = _repository.Add(_postHabitLog);
                if (attempt == false)
                {
                    return new UnitOfWorkResult<bool>(false, false);
                }
                _successfullyAddedLogDates.Add(date);
            }

            bool finalAttempt = _repository.Add(_postHabitLog);

            return new UnitOfWorkResult<bool>(finalAttempt, finalAttempt);
        }

        public UnitOfWorkResult<bool?> rollback()
        {
            bool allSuccessful = true;

            for (int i = 0; i < _successfullyAddedLogDates.Count; i++)
            {
                bool success = _repository.DeleteByHabitIdAndStartDate(_postHabitLog.Habit_id, _successfullyAddedLogDates[i]);

                if (success == false)
                {
                    allSuccessful = false;
                }
            }

            return new UnitOfWorkResult<bool?>(allSuccessful, null);
        }

        private int CalculateAmountOfHabitsToAdd(HabitLog? habitLog)
        {
            if (habitLog != null)
            {
                DateTime lastLogStartDate = habitLog.Start_date;

                int differenceInDays = _postHabitLog.Start_date.Subtract(lastLogStartDate).Days;

                Console.WriteLine($"DIFFERENCE IN DAYS {differenceInDays}");

                int daysForMissingLogs = differenceInDays - _postHabitLog.Length_in_days;

                Console.WriteLine($"DAYS FOR MISSING LOGS {daysForMissingLogs}");

                if (daysForMissingLogs > _postHabitLog.Length_in_days)
                {
                    return daysForMissingLogs / _postHabitLog.Length_in_days;
                }
            }
            return 0;
        }
    }
}
