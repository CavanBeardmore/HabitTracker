using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PostUser
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public PostUser(string username, string email, string password) 
        {
            Username = username;
            Email = email;
            Password = password;
        }
    }
}
