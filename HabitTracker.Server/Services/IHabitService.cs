using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Services
{
    public interface IHabitService
    {
        IReadOnlyCollection<Habit?> GetAllByUserId(int user_id);
        Habit? GetById(int habitId, int userId);
        Habit? Add(int userId, PostHabit habit);
        bool Delete(int habitId, int userid);
        Habit? Update(int userId, PatchHabit habit);
    }
}
