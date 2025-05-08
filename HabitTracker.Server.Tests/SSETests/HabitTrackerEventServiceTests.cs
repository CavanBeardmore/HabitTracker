using HabitTracker.Server.SSE;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Channels;

namespace HabitTracker.Server.Tests.SSETests
{
    public class HabitTrackerEventServiceTests
    {
        private readonly Mock<ILogger<HabitTrackerEventService>> _mockLogger;

        public HabitTrackerEventServiceTests()
        {
            _mockLogger = new Mock<ILogger<HabitTrackerEventService>>();
        }
        [Fact]
        public void IsActiveForUser_ShouldReturnFalseWhenThereIsNoChannelForUser()
        {
            var eventService = new HabitTrackerEventService(_mockLogger.Object);

            var result = eventService.IsActiveForUser(1234);

            Assert.False(result);
        }

        [Fact]
        public void IsActiveForUser_ShouldReturnTrueWhenThereIsAChannelForUser()
        {
            var eventService = new HabitTrackerEventService(_mockLogger.Object);
            eventService.CreateChannelForUser(1234);

            var result = eventService.IsActiveForUser(1234);

            Assert.True(result);
        }

        [Fact]
        public void CreateChannelForAUser_ShouldAddChannelToDictionaryWithUserId()
        {
            var eventService = new HabitTrackerEventService(_mockLogger.Object);
            Assert.Null(eventService.GetReaderForUser(1234));

            eventService.CreateChannelForUser(1234);

            var channel = eventService.GetReaderForUser(1234);

            Assert.NotNull(channel);
        }

        [Fact]
        public void DeleteChannelForAUser_ShouldAddChannelToDictionaryWithUserId()
        {
            var eventService = new HabitTrackerEventService(_mockLogger.Object);
            eventService.CreateChannelForUser(1234);

            var channel1 = eventService.GetReaderForUser(1234);
            Assert.NotNull(channel1);
            eventService.DeleteChannelForUser(1234);

            var channel2 = eventService.GetReaderForUser(1234);
            Assert.Null(channel2);
        }

        [Fact]
        public void Reader_ShouldHaveNoEvents()
        {
            var eventService = new HabitTrackerEventService(_mockLogger.Object);
            eventService.CreateChannelForUser(1234);
            var result = eventService.GetReaderForUser(1234)!.Count;

            Assert.True(result == 0);
        }

        [Fact]
        public void AddEvent_ShouldAddEventToChannel()
        {
            var eventService = new HabitTrackerEventService(_mockLogger.Object);
            eventService.CreateChannelForUser(1234);
            eventService.AddEvent(1234, new HabitTrackerEvent(HabitTrackerEventTypes.LOGGED_IN, 1234));

            var result = eventService.GetReaderForUser(1234)!.Count;

            Assert.True(result == 1);
        }
    }
}
