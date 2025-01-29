using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PatchHabitLog
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required] 
        public int HabitId { get; set; }
        [Required]
        public bool Habit_logged { get; set; }

        public PatchHabitLog(int id, int userId, int habitId, bool habitLogged)
        {
            Id = id;
            UserId = userId;
            HabitId = habitId;
            Habit_logged = habitLogged;
        }
    }
}
