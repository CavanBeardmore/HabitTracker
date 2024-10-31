using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Repository
{
    public interface IHabitLogRepository
    {
        IEnumerable<HabitLog> GetAllByHabitId(int id);
        HabitLog? GetById(int id);
        bool Add(PostHabitLog habitLog);
        bool Update(PatchHabitLog habitLog);
        bool Delete(int id);
    }
}
