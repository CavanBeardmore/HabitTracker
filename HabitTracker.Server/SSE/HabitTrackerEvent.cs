namespace HabitTracker.Server.SSE
{
    public class HabitTrackerEvent
    {
        public string EventType { get; }
        public object Data { get; }
        public HabitTrackerEvent(string eventType, object data) 
        {
            EventType = eventType;
            Data = data;
        }
    }
}
