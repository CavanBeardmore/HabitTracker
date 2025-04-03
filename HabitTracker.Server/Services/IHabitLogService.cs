using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services
{
    public interface IHabitLogService
    {
        IReadOnlyCollection<HabitLog?> GetAllByHabitId(int id, int userId, int pageNumber);

        HabitLog? GetById(int habitLogId, int userId);

        HabitLog? Add(PostHabitLog habitLog, int userId);

        bool Update(PatchHabitLog habitLog);

        bool Delete(int habitLogId, int userId);

    }
}
