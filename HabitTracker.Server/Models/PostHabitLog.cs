using HabitTracker.Server.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PostHabitLog
    {
        [Required]
        public int User_id { get; set; }
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

        public PostHabitLog(int userId, int habitId, DateTime startDate, bool habitLogged, int lengthInDays) 
        {
            User_id = userId;
            Habit_id = habitId;
            Start_date = startDate;
            Habit_logged = habitLogged;
            Length_in_days = lengthInDays;
        }
    }
}
