namespace HabitTracker.Server.Services
{
    public interface IRateLimitService
    {
        void CheckRateLimitForIpAddress(string ipAddress);
    }
}
