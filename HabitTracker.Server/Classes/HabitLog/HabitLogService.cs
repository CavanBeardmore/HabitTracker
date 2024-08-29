namespace HabitTracker.Server.Classes.HabitLog
{
    public class HabitLogService
    {
        private readonly IHabitLogRepository _habitLogRepository;

        public HabitLogService(IHabitLogRepository habitLogRepository)
        {
            _habitLogRepository = habitLogRepository;
        }

        public HabitLog GetById(int habitId)
        {
            return _habitLogRepository.GetById(habitId);
        }

        public IEnumerable<HabitLog> GetAllHabitlogsByHabitId(int habitId)
        {
            return _habitLogRepository.GetAllByHabitId(habitId);
        }

        public void AddHabitLog(HabitLog habitlog)
        {
            _habitLogRepository.Add(habitlog);
        }

        public void UpdateHabitLog(HabitLog habitLog)
        {
            _habitLogRepository.Update(habitLog);
        }

        public void DeleteHabitLog(int habitlogId)
        {
            _habitLogRepository.Delete(habitlogId);
        }
    }
}
