using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Services.Responses.UserResponses
{
    public class GetUserByUsernameResponse : IServiceResponseWithDataAndStatusCode<User?>
    {
        public bool Success { get; }
        public User? Data { get; }
        public string? Error { get; }
        public EStatusCodes? StatusCode { get; }

        public GetUserByUsernameResponse(bool success, User? data, string? error, EStatusCodes statusCode)
        {
            Success = success;
            Data = data;
            Error = error;
            StatusCode = statusCode;
        }
    }
}
