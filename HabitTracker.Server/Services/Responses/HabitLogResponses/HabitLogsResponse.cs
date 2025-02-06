namespace HabitTracker.Server.Services.Responses.HabitLogResponses
{
    public class HabitLogsResponse : IServiceResponse
    {
        public bool Success { get; }
        public string? Error { get; }

        public HabitLogsResponse(bool success, string? error)
        {
            Success = success;
            Error = error;
        }
    }
}
