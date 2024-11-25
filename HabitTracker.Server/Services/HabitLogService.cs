using HabitTracker.Server.Repository;

namespace HabitTracker.Server.Services
{
    public class HabitLogService
    {
        private readonly IHabitLogRepository _habitLogRepository;

        public HabitLogService(IHabitLogRepository habitLogRepository)
        {
            _habitLogRepository = habitLogRepository;
        }
    }
}
