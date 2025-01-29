using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PatchHabit
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        [Required]
        public int UserId { get; set; }

        public PatchHabit(int id, string? name, int userId)
        {
            Id = id;
            Name = name;
            UserId = userId;
        }
    }
}
