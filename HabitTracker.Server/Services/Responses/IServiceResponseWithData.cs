namespace HabitTracker.Server.Services.Responses
{
    public interface IServiceResponseWithData<T> : IServiceResponse
    {
        T? Data { get; }
    }
}
