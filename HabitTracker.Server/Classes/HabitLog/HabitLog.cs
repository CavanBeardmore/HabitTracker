namespace HabitTracker.Server.Classes.HabitLog
{
    public class HabitLog
    {

        public int habitLog_id { get; set; }
        public int habit_id { get; set; }
        public DateTime start_date { get; set; }
        public bool habit_logged { get; set; }
        public int period_type { get; set; }
        public HabitLog(int habitlog_id, int habit_id, DateTime start_date, bool habit_logged, int period_type)
        {
            this.habitLog_id = habitlog_id;
            this.habit_id = habit_id;
            this.start_date = start_date;
            this.habit_logged = habit_logged;
            this.period_type = period_type;
        }
    }
}
