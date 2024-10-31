namespace HabitTracker.Server.Models
{
    public class PostHabitLog
    {
        public int? Habit_id { get; set; }
        public DateTime? Start_date { get; set; }
        public bool? Habit_logged { get; set; }
        public int? Length_in_days { get; set; }
    }
}
