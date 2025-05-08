using System.Threading.Channels;
using System.Collections.Concurrent;

namespace HabitTracker.Server.SSE
{
    public class HabitTrackerEventService : IEventService<HabitTrackerEvent>
    {
        private readonly ConcurrentDictionary<int, Channel<HabitTrackerEvent>> _channelCollection;
        private readonly ILogger<HabitTrackerEventService> _logger;

        public HabitTrackerEventService(ILogger<HabitTrackerEventService> logger)
        {
            _channelCollection = new ConcurrentDictionary<int, Channel<HabitTrackerEvent>>();
            _logger = logger;
        }

        public bool IsActiveForUser(int userId)
        {
            return _channelCollection.TryGetValue(userId, out _);
        }

        public bool CreateChannelForUser(int userId)
        {
            if (_channelCollection.TryGetValue(userId, out _))
            {
                return true;
            }
            return _channelCollection.TryAdd(userId, Channel.CreateUnbounded<HabitTrackerEvent>());
        }

        public bool DeleteChannelForUser(int userId)
        {
            if (_channelCollection.TryGetValue(userId, out _) == false)
            {
                return true;
            }
            return _channelCollection.TryRemove(userId, out _);
        }

        public ChannelReader<HabitTrackerEvent>? GetReaderForUser(int userId) 
        {
            _channelCollection.TryGetValue(userId, out var channel);

            if (channel == null)
            {
                return null;
            }

            return channel.Reader;
        }

        public void AddEvent(int userId, HabitTrackerEvent sseEvent)
        {
            if (_channelCollection.TryGetValue(userId, out var channel)) {
                _logger.LogInformation("HabitTrackerEventService - AddEvent - adding event {@Event}", sseEvent);

                channel.Writer.TryWrite(sseEvent);
            }
        }
    }
}
