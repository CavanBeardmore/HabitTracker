using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services
{
    public interface IHabitLogService
    {
        IReadOnlyCollection<HabitLog?> GetAllByHabitId(int id, int userId, int pageNumber);

        HabitLog? GetById(int habitLogId, int userId);

        HabitLog? GetMostRecentByHabitId(int habitId, int userId);

        Tuple<Habit, HabitLog>? Add(PostHabitLog habitLog, int userId);

        HabitLog? Update(PatchHabitLog habitLog);

        bool Delete(int habitLogId, int userId);

    }
}
