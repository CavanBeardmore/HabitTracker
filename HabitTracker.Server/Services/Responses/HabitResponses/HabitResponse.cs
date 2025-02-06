namespace HabitTracker.Server.Services.Responses.HabitResponses
{
    public class HabitResponse : IServiceResponse
    {
        public bool Success { get; }
        public string? Error { get; }

        public HabitResponse(bool success, string? error)
        {
            Success = success;
            Error = error;
        }
    }
}
