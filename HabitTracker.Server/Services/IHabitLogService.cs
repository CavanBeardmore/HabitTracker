using HabitTracker.Server.Models;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services
{
    public interface IHabitLogService
    {
        IServiceResponseWithData<IReadOnlyCollection<HabitLog?>> GetAllByHabitId(int id, int userId, int pageNumber);

        IServiceResponseWithData<HabitLog?> GetById(int habitLogId, int userId);

        IServiceResponse Add(PostHabitLog habitLog);

        IServiceResponse Update(PatchHabitLog habitLog);

        IServiceResponse Delete(int habitLogId, int userId);

    }
}
