namespace HabitTracker.Server.Classes.Habit
{
    public class HabitService
    {
        private readonly IHabitRepository _habitRepository;

        public HabitService(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }

        public Habit GetHabitByHabitId(int habitId)
        {
            return _habitRepository.GetById(habitId);
        }

        public IEnumerable<Habit> GetAllHabitsByUsername(string username)
        {
            return _habitRepository.GetAllByUsername(username);
        }

        public void AddHabit(Habit habit)
        {
            _habitRepository.Add(habit);
        }

        public void UpdateHabit(Habit habit)
        {
            _habitRepository.Update(habit);
        }

        public void DeleteHabit(int habitId)
        {
            _habitRepository.Delete(habitId);
        }
    }
}
