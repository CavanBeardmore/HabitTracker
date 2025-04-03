using HabitTracker.Server.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PostHabitLog
    {
        [Required]
        public int Habit_id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Start_date { get; set; }
        [Required]
        public bool Habit_logged { get; set; }
        [Required]
        [ValidLengthInDays]
        public int Length_in_days { get; set; }

        public PostHabitLog(int Habit_id, DateTime Start_date, bool Habit_logged, int Length_in_days) 
        {
            this.Habit_id = Habit_id;
            this.Start_date = Start_date;
            this.Habit_logged = Habit_logged;
            this.Length_in_days = Length_in_days;
        }
    }
}
