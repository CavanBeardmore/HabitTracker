namespace HabitTracker.Server.Classes.HabitLog
{
    public class CreateHabitLogRequest
    {
        public int habitLog_id { get; set; }
        public int habit_id { get; set; }
        public DateTime start_date { get; set; }
        public bool habit_logged { get; set; }
        public int period_type { get; set; }
    }
}
