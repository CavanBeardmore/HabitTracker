namespace HabitTracker.Server.Models
{
    public class PatchUser
    {
        public string OldUsername { get; set; }
        public string NewUsername { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
