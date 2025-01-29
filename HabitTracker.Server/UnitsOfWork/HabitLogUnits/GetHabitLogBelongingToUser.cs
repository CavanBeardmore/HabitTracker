using HabitTracker.Server.DTOs;
using HabitTracker.Server.Repository;

namespace HabitTracker.Server.UnitsOfWork.HabitLogUnits
{
    public class GetHabitLogBelongingToUser : IUnitOfWork<HabitLog?>
    {
        private readonly int _habitLogId;
        private readonly int _userId;
        private readonly IHabitLogRepository _repository;
        public GetHabitLogBelongingToUser(int habitLogId, int userId, IHabitLogRepository repository) 
        {
            _habitLogId = habitLogId;
            _userId = userId;
            _repository = repository;
        }

        public UnitOfWorkResult<HabitLog?> execute()
        {
            var habitlogs = _repository.GetById(_habitLogId, _userId);

            if (habitlogs == null)
            {
                return new UnitOfWorkResult<HabitLog?>(false, null);
            }

            return new UnitOfWorkResult<HabitLog?>(true, habitlogs);
        }
        public UnitOfWorkResult<HabitLog?> rollback()
        {
            return new UnitOfWorkResult<HabitLog?>(true, null);
        }
    }
}
