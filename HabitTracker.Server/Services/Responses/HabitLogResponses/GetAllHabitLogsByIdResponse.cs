using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services.Responses.HabitLogResponses
{
    public class GetAllHabitLogsByIdResponse : IServiceResponseWithData<IReadOnlyCollection<HabitLog?>>
    {
        public bool Success { get; }
        public IReadOnlyCollection<HabitLog?> Data { get; }
        public string? Error { get; }

        public GetAllHabitLogsByIdResponse(bool success, IReadOnlyCollection<HabitLog?> habitLogs, string? error)
        {
            Success = success;
            Data = habitLogs;
            Error = error;
        }
    }
}
