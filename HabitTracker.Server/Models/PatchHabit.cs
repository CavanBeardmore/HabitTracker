using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PatchHabit
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public PatchHabit(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
