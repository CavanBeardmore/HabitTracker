namespace HabitTracker.Server.Classes.User
{
    public class User
    {
        public int user_id {  get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public User(int user_id, string username, string email, string password = "") 
        {
            this.user_id = user_id;
            this.username = username;
            this.email = email;
            this.password = password;
        }
    }
}
