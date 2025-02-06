using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services.Responses.HabitLogResponses
{
    public class GetHabitLogByIdResponse : IServiceResponseWithData<HabitLog?>
    {
        public bool Success { get; }
        public HabitLog? Data { get; }
        public string? Error { get; }

        public GetHabitLogByIdResponse(bool success, HabitLog? habitLog, string? error)
        {
            Success = success;
            Data = habitLog;
            Error = error;
        }
    }
}
