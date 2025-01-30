using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Database.Entities
{
    public class TUser
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public List<THabit> Habits { get; set; } = new List<THabit>();
    }
}
