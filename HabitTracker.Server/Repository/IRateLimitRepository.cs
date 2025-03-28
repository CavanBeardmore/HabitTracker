using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Repository
{
    public interface IRateLimitRepository
    {
        Rate? GetRate(string ipAddress);
        Rate? AddRate(uint count, string ipAddress, DateTime ttl);
        bool UpdateRateCount(uint count, string ipAddress);
        bool UpdateRateCountAndTtl(uint count, string ipAddress, DateTime date);
    }
}
