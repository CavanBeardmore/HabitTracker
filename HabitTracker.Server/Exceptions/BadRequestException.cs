namespace HabitTracker.Server.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message) { }
    }
}
