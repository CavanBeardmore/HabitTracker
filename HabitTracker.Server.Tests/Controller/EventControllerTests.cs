using HabitTracker.Server.Controllers;
using HabitTracker.Server.SSE;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Server.Tests.Controller
{
    public class EventControllerTests
    {
        private readonly EventController _controller;
        private readonly Mock<IEventService<HabitTrackerEvent>> _mockEventService;
        private readonly Mock<ILogger<EventController>> _mockLogger;

        public EventControllerTests()
        {
            {
                _mockEventService = new Mock<IEventService<HabitTrackerEvent>>();
                _mockLogger = new Mock<ILogger<EventController>>();
                _controller = new EventController(_mockEventService.Object, _mockLogger.Object);
            }
        }
    }
}