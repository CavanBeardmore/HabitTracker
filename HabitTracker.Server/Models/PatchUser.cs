using HabitTracker.Server.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Models
{
    public class PatchUser
    {
        [Required]
        [StringLength(100)]
        public string OldUsername { get; set; }

        [StringLength(100)]
        public string? NewUsername { get; set; }

        [OptionalEmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        public PatchUser(string oldUsername, string oldPassword, string newUsername = null, string email = null, string newPassword = null) 
        {
            OldUsername = oldUsername;
            NewUsername = newUsername;
            Email = email;
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
}
