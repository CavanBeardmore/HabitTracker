using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PatchHabitLog
    {
        [Required]
        public int Id { get; set; }
        [Required] 
        public bool Habit_logged { get; set; }

        public PatchHabitLog(int id, bool habitLogged)
        {
            Id = id;
            Habit_logged = habitLogged;
        }
    }
}
