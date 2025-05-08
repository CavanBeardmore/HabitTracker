using HabitTracker.Server.SSE;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Server.Tests.SSETests
{
    public class HabitTrackerEventTests
    {
        private readonly HabitTrackerEvent _event;

        public HabitTrackerEventTests()
        {
            _event = new HabitTrackerEvent(HabitTrackerEventTypes.HABIT_ADDED, 1234);
        }

        [Fact]
        public void EventType_ShouldBeHabitAdded()
        {
            var result = _event.EventType;

            Assert.True(result == HabitTrackerEventTypes.HABIT_ADDED);
        }

        [Fact]
        public void Data_ShouldBe1234()
        {
            var result = _event.Data;

            Assert.True((int)_event.Data == 1234);
        }
    }
}
