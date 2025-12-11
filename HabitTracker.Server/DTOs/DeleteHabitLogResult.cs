namespace HabitTracker.Server.DTOs;

public record DeleteHabitLogResult(bool Success, HabitLog? HabitLog);