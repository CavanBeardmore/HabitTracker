using System.Threading.Channels;

namespace HabitTracker.Server.SSE
{
    public interface IEventService<T>
    {
        bool IsActiveForUser(int userId);
        bool CreateChannelForUser(int userId);
        bool DeleteChannelForUser(int userId);
        ChannelReader<T>? GetReaderForUser(int userId);
        void AddEvent(int userId, T sseEvent);
    }
}
