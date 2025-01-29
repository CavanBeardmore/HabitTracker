using HabitTracker.Server.DTOs;
using HabitTracker.Server.Repository;

namespace HabitTracker.Server.UnitsOfWork.HabitLogUnits
{
    public class DeleteHabitLogBelongingToUser
    {
        private readonly int _habitLogId;
        private readonly int _userId;
        private readonly IHabitLogRepository _repository;
        public DeleteHabitLogBelongingToUser(int habitLogId, int userId, IHabitLogRepository repository)
        {
            _habitLogId = habitLogId;
            _userId = userId;
            _repository = repository;
        }

        public UnitOfWorkResult<bool?> execute()
        {
            return new UnitOfWorkResult<bool?>(_repository.Delete(_habitLogId, _userId), null);
        }
        public UnitOfWorkResult<bool?> rollback()
        {
            return new UnitOfWorkResult<bool?>(true, null);
        }

    }
}
