using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services.Responses.HabitResponses
{
    public class GetHabitByIdResponse : IServiceResponseWithData<Habit?>
    {
        public bool Success { get; }
        public Habit? Data { get; }
        public string? Error { get; }

        public GetHabitByIdResponse(bool success, Habit? habitLog, string? error)
        {
            Success = success;
            Data = habitLog;
            Error = error;
        }
    }
}
