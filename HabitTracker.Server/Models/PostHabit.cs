using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PostHabit
    {
        [Required]
        public int User_id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public PostHabit(int user_id, string name)
        {
            User_id = user_id;
            Name = name;
        }
    }
}
