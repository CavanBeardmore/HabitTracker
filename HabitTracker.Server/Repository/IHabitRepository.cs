using System.Data.SQLite;
using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Repository
{
    public interface IHabitRepository
    {
        IEnumerable<Habit> GetAllByUserId(int user_id);
        Habit? GetById(int id);
        bool Add(PostHabit habit);
        bool Update(PatchHabit habit);
        bool Delete(int id);
    }
}
