using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class AuthUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
