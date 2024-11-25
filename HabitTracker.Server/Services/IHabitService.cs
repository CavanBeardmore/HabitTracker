using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Services
{
    public interface IHabitService
    {
        IEnumerable<Habit> GetAllByUserId(int user_id);
        Habit? GetById(int id);
        bool Add(PostHabit habit);
        bool Delete(int id);
        bool Update(PatchHabit habit);
    }
}
