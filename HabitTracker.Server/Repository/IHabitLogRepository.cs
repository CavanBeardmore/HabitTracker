using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Repository
{
    public interface IHabitLogRepository
    {
        IReadOnlyCollection<HabitLog> GetAllByHabitId(int id, int userId, int pageNumber);
        HabitLog? GetById(int habitLogId, int userId);
        HabitLog? GetByHabitIdAndStartDate(int habitId, int userId, DateTime date);
        HabitLog? GetMostRecentHabitLog(int habitId, int userId);
        HabitLog? Add(PostHabitLog habitLog);
        bool Update(PatchHabitLog habitLog);
        bool Delete(int habitLogId, int userId);

        bool DeleteByHabitIdAndStartDate(int habitId, DateTime startDate);
    }
}
