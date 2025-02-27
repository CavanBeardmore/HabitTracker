namespace HabitTracker.Server.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message): base(message) { }
    }
}
