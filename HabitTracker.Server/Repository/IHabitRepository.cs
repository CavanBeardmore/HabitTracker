using System.Data.SQLite;
using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Repository
{
    public interface IHabitRepository
    {
        IReadOnlyCollection<Habit> GetAllByUserId(int user_id);
        Habit? GetById(int habitId, int userId);
        bool Add(PostHabit habit);
        bool Update(PatchHabit habit);
        bool Delete(int habitId, int userId);
    }
}
