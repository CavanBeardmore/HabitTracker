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
        public string NewUsername { get; set; }

        [OptionalEmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
