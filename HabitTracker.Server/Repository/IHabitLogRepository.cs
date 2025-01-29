using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Repository
{
    public interface IHabitLogRepository
    {
        IReadOnlyCollection<HabitLog> GetAllByHabitId(int id, int userId, int pageNumber);
        HabitLog? GetById(int habitLogId, int userId);

        HabitLog? GetMostRecentHabitLog(int habitId, int userId);
        bool Add(PostHabitLog habitLog);
        bool Update(PatchHabitLog habitLog);
        bool Delete(int habitLogId, int userId);

        bool DeleteByHabitIdAndStartDate(int habitId, DateTime startDate);
    }
}
