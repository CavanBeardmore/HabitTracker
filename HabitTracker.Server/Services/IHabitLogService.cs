using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services
{
    public interface IHabitLogService
    {
        Tuple<IReadOnlyCollection<HabitLog>, bool>? GetAllByHabitId(int id, int userId, uint pageNumber);

        HabitLog? GetById(int habitLogId, int userId);

        HabitLog? GetMostRecentByHabitId(int habitId, int userId);

        AddedHabitLogResult? Add(PostHabitLog habitLog, int userId);

        HabitLog? Update(PatchHabitLog habitLog);

        bool Delete(int habitLogId, int userId);

    }
}
