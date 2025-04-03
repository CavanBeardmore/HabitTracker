namespace HabitTracker.Server.DTOs
{
    public class HabitLog
    {
        public int Id { get; set; }
        public int Habit_id { get; set; }
        public DateTime Start_date { get; set; }
        public bool Habit_logged { get; set; }
        public int LengthInDays { get; set; }

        public HabitLog(int id, int habit_id, DateTime start_date, bool habit_logged, int lengthInDays)
        {
            Id = id;
            Habit_id = habit_id;
            Start_date = start_date;
            Habit_logged = habit_logged;
            LengthInDays = lengthInDays;
        }
    }
}
