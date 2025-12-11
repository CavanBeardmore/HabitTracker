namespace HabitTracker.Server.DTOs;

public record PaginatedHabitLogs(IReadOnlyCollection<HabitLog> HabitLogs, bool HasMore);