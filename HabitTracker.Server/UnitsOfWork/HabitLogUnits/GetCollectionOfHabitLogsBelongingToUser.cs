using HabitTracker.Server.DTOs;
using HabitTracker.Server.Repository;

namespace HabitTracker.Server.UnitsOfWork.HabitLogUnits
{
    public class GetCollectionOfHabitLogsBelongingToUser : IUnitOfWork<IReadOnlyCollection<HabitLog>>
    {
        private readonly int _habitLogId;
        private readonly int _userId;
        private readonly int _pageNumber;
        private readonly IHabitLogRepository _repository;
        public GetCollectionOfHabitLogsBelongingToUser(int habitLogId, int userId, int pageNumber, IHabitLogRepository repository)
        {
            _habitLogId = habitLogId;
            _userId = userId;
            _pageNumber = pageNumber;
            _repository = repository;
        }

        public UnitOfWorkResult<IReadOnlyCollection<HabitLog>> execute()
        {
            IReadOnlyCollection<HabitLog> habitlogs = _repository.GetAllByHabitId(_habitLogId, _userId, _pageNumber);

            if (habitlogs.Count == 0)
            {
                return new UnitOfWorkResult<IReadOnlyCollection<HabitLog>>(false, []);
            }

            return new UnitOfWorkResult<IReadOnlyCollection<HabitLog>>(true, habitlogs);
        }
        public UnitOfWorkResult<IReadOnlyCollection<HabitLog> >rollback()
        {
            return new UnitOfWorkResult<IReadOnlyCollection<HabitLog>>(true, []);
        }
    }
}
