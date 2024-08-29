using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Classes.Habit
{
    public class CreateHabitRequest
    {
        public int HabitId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
