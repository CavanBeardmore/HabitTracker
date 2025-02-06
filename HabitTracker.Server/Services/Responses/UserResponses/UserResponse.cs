namespace HabitTracker.Server.Services.Responses.UserResponses
{
    public class UserResponse : IServiceResponseWithStatusCode
    {
        public bool Success { get; }
        public string? Error { get; }
        public EStatusCodes? StatusCode { get; }

        public UserResponse(bool success, string? error, EStatusCodes? statusCode = EStatusCodes.INTERNAL_SERVER_ERROR)
        {
            Success = success;
            Error = error;
            StatusCode = statusCode;
        }
    }
}
