namespace HabitTracker.Server.Exceptions
{
    public class TooManyRequestsException : AppException
    {
        public TooManyRequestsException(string message) : base(message) { }
    }
}
