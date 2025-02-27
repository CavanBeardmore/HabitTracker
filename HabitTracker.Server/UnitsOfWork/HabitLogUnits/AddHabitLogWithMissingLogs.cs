using HabitTracker.Server.Database.Entities;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.UnitsOfWork.HabitLogUnits
{
    public class AddHabitLogWithMissingLogs
    {
        private List<HabitLog> _successfullyAddedHabitLogs = new List<HabitLog>();
        private readonly PostHabitLog _postHabitLog;
        private readonly IHabitLogRepository _repository;
        public AddHabitLogWithMissingLogs(PostHabitLog postHabitLog, IHabitLogRepository repository)
        {
            _postHabitLog = postHabitLog;
            _repository = repository;
        }

        public UnitOfWorkResult<IReadOnlyCollection<HabitLog?>> execute()
        {
            var habitlog = _repository.GetMostRecentHabitLog(_postHabitLog.Habit_id, _postHabitLog.User_id);

            int habitsToAdd = CalculateAmountOfHabitsToAdd(habitlog);

            for (int i = habitsToAdd; i > 0; i--)
            {
                DateTime date = _postHabitLog.Start_date.AddDays(-i);

                HabitLog? habitLog = _repository.Add(_postHabitLog);

                bool validHabitLog = handleHabitLogAddResult(habitLog);
                if (validHabitLog == false)
                {
                    return new UnitOfWorkResult<IReadOnlyCollection<HabitLog?>>(false, new List<HabitLog>());
                }
;            }

            HabitLog? finalHabitLog = _repository.Add(_postHabitLog);
            bool validFinalHabitLog = handleHabitLogAddResult(finalHabitLog);
            if (validFinalHabitLog == false)
            {
                return new UnitOfWorkResult<IReadOnlyCollection<HabitLog?>>(false, new List<HabitLog>());
            }

            return new UnitOfWorkResult<IReadOnlyCollection<HabitLog?>>(true, _successfullyAddedHabitLogs);
        }

        public UnitOfWorkResult<bool?> rollback()
        {
            bool allSuccessful = true;

            for (int i = 0; i < _successfullyAddedHabitLogs.Count; i++)
            {
                bool success = _repository.DeleteByHabitIdAndStartDate(_postHabitLog.Habit_id, _successfullyAddedHabitLogs[i].Start_date);

                if (success == false)
                {
                    allSuccessful = false;
                }
            }

            return new UnitOfWorkResult<bool?>(allSuccessful, null);
        }

        private bool handleHabitLogAddResult(HabitLog? habitLog)
        {
            if (habitLog != null)
            {
                _successfullyAddedHabitLogs.Add(habitLog);
                return true;
            }
            return false;
        }

        private int CalculateAmountOfHabitsToAdd(HabitLog? habitLog)
        {
            if (habitLog != null)
            {
                DateTime lastLogStartDate = habitLog.Start_date;

                int differenceInDays = _postHabitLog.Start_date.Subtract(lastLogStartDate).Days;

                int daysForMissingLogs = differenceInDays - _postHabitLog.Length_in_days;

                if (daysForMissingLogs > _postHabitLog.Length_in_days)
                {
                    return daysForMissingLogs / _postHabitLog.Length_in_days;
                }
            }
            return 0;
        }
    }
}
