using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PostHabit
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public PostHabit(string name)
        {
            Name = name;
        }
    }
}
