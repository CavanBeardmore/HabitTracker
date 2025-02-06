namespace HabitTracker.Server.Services.Responses
{

    public interface IServiceResponseWithStatusCode : IServiceResponse
    {
        EStatusCodes? StatusCode { get; }
    }
}
