using HabitTracker.Server.Models;

namespace HabitTracker.Server.DTOs
{
    public record AddHabitLogData(PostHabitLog Habit, int UserId);
}
