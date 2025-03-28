using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class AuthUser
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public AuthUser() { }

        public AuthUser(string username, string password) 
        {
            Username = username;
            Password = password;
        }
    }
}
