namespace HabitTracker.Server.Services
{
    public interface IRateLimitService
    {
        bool HasIpAddressBeenLimited(string ipAddress);
        void CheckRateLimitForIpAddress(string ipAddress);
    }
}
