using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Database.Entities
{
    public class THabitLog
    {
        [Key]
        public int Id { get; set; }
        public int Habit_id { get; set; }
        public DateTime Start_date { get; set; }
        public bool Habit_logged { get; set; }
        public int Length_in_days { get; set; }
        public THabit Habit { get; set; }
    }
}
