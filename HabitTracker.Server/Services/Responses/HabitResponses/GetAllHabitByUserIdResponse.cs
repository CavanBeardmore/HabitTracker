using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services.Responses.HabitResponses
{
    public class GetAllHabitByUserIdResponse : IServiceResponseWithData<IReadOnlyCollection<Habit>>
    {
        public bool Success { get; }
        public IReadOnlyCollection<Habit?> Data { get; }
        public string? Error { get; }

        public GetAllHabitByUserIdResponse(bool success, IReadOnlyCollection<Habit> habitLogs, string? error)
        {
            Success = success;
            Data = habitLogs;
            Error = error;
        }
    }
}
