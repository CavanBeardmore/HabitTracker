using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Classes.Habit
{
    public class CreateHabitRequest
    {
        public int HabitId { get; set; }
        public string username { get; set; }
        public string Name { get; set; }
    }
}
