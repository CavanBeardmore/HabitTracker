namespace HabitTracker.Server.Models
{
    public class PostHabit
    {
        public int User_id { get; set; }
        public string Name { get; set; }

        public PostHabit(int user_id, string name)
        {
            User_id = user_id;
            Name = name;
        }
    }
}
