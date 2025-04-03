using System.Data.SQLite;
using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Repository
{
    public interface IHabitRepository
    {
        IReadOnlyCollection<Habit> GetAllByUserId(int user_id);
        Habit? GetById(int habitId, int userId);
        Habit? Add(int userId, PostHabit habit);
        bool Update(int userId, PatchHabit habit);
        bool Delete(int habitId, int userId);
    }
}
