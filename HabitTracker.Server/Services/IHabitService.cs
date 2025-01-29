using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Services
{
    public interface IHabitService
    {
        IServiceResponseWithData<IReadOnlyCollection<Habit>> GetAllByUserId(int user_id);
        IServiceResponseWithData<Habit?> GetById(int habitId, int userId);
        IServiceResponse Add(PostHabit habit);
        IServiceResponse Delete(int habitId, int userid);
        IServiceResponse Update(PatchHabit habit);
    }
}
