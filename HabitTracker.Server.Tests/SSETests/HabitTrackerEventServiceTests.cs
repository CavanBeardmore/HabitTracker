using HabitTracker.Server.SSE;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Server.Tests.SSETests
{
    public class HabitTrackerEventServiceTests
    {
        private readonly Mock<ILogger<HabitTrackerEventService>> _mockLogger;
        private readonly HabitTrackerEventService _eventService;

        public HabitTrackerEventServiceTests()
        {
            _mockLogger = new Mock<ILogger<HabitTrackerEventService>>();
            _eventService = new HabitTrackerEventService(_mockLogger.Object);
        }

        [Fact]
        public void Reader_ShouldHaveNoEvents()
        {
            var result = _eventService.Reader.Count;

            Assert.True(result == 0);
        }

        [Fact]
        public void AddEvent_ShouldAddEventToChannel()
        {
            _eventService.AddEvent(new HabitTrackerEvent(HabitTrackerEventTypes.LOGGED_IN, 1234));

            var result = _eventService.Reader.Count;

            Assert.True(result == 1);
        }
    }
}
