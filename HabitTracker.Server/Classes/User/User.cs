namespace HabitTracker.Server.Classes.User
{
    public class User
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public User(string username, string email, string password = "") 
        {
            this.username = username;
            this.email = email;
            this.password = password;
        }
    }
}
