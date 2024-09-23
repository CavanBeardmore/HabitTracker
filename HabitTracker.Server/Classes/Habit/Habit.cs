namespace HabitTracker.Server.Classes.Habit
{
    public class Habit
    {

        public int habit_id { get; set; }
        public string username { get; set; }
        public string name { get; set; }

        public Habit(int habit_id, string username, string name)
        {
            this.habit_id = habit_id;
            this.username = username;
            this.name = name;
        }
    }
}
