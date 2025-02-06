namespace HabitTracker.Server.Services.Responses
{
    public interface IServiceResponseWithDataAndStatusCode<T> : IServiceResponseWithData<T>, IServiceResponseWithStatusCode;
}
