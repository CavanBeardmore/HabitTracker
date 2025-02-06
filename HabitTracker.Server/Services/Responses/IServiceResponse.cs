namespace HabitTracker.Server.Services.Responses
{

    public interface IServiceResponse
    {
        bool Success { get; }
        string? Error { get; }
    }
}
