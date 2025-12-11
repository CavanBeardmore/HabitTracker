using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services
{
    public interface IHabitLogService
    {
        PaginatedHabitLogs? GetAllByHabitId(int id, int userId, uint pageNumber);

        HabitLog? GetById(int habitLogId, int userId);

        HabitLog? GetMostRecentByHabitId(int habitId, int userId);

        AddedHabitLogResult? Add(PostHabitLog habitLog, int userId);

        HabitLog? Update(PatchHabitLog habitLog, int userId);

        DeleteHabitLogResult Delete(int habitLogId, int userId);

    }
}
