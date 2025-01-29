namespace HabitTracker.Server.Services
{
    public enum EStatusCodes
    {
        OK = 200,
        CREATED = 201,
        BAD_REQUEST = 400,
        UNAUTHORIZED = 401,
        FORBIDDEN = 403,
        NOT_FOUND = 404,
        CONFLICT = 409,
        INTERNAL_SERVER_ERROR = 500
    }

    public interface IServiceResponseWithData<T> : IServiceResponse
    {
        bool Success { get; }
        T? Data { get; }
        string Error { get; }
    }

    public interface IServiceResponseWithDataAndStatusCode<T> : IServiceResponseWithData<T>, IServiceResponseWithStatusCode;

    public interface IServiceResponse
    {
        bool Success { get; }
        string? Error { get; }
    }

    public interface IServiceResponseWithStatusCode : IServiceResponse
    {
        bool Success { get; }
        string? Error { get; }
        EStatusCodes? StatusCode { get; }
    }
}
