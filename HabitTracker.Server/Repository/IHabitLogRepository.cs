using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using System.Data.Common;

namespace HabitTracker.Server.Repository
{
    public interface IHabitLogRepository
    {
        IReadOnlyCollection<HabitLog> GetAllByHabitId(int id, int userId, int pageNumber);
        HabitLog? GetById(int habitLogId, int userId);
        HabitLog? GetByHabitIdAndStartDate(int habitId, int userId, DateTime date);
        HabitLog? GetMostRecentHabitLog(int habitId, int userId, DbConnection connection, DbTransaction transaction);
        HabitLog? Add(PostHabitLog habitLog, DbConnection connection, DbTransaction transaction);
        HabitLog? Update(PatchHabitLog habitLog);
        bool Delete(int habitLogId, int userId);

        bool DeleteByHabitIdAndStartDate(int habitId, DateTime startDate);
    }
}
