using System.Data.Common;
using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Storage;

namespace HabitTracker.Server.Repository
{
    public interface IHabitRepository
    {
        IStorage SqliteFacade { get; }
        IReadOnlyCollection<Habit> GetAllByUserId(int user_id);
        Habit? GetById(int habitId, int userId, DbConnection? connection, DbTransaction? transaction);
        Habit? Add(int userId, PostHabit habit);
        bool Update(int userId, PatchHabit habit, DbConnection? connection, DbTransaction? transaction);
        bool Delete(int habitId, int userId);
    }
}
