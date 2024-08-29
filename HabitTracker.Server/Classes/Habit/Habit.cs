namespace HabitTracker.Server.Classes.Habit
{
    public class Habit
    {

        public int habit_id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }

        public Habit(int habit_id, int user_id, string name)
        {
            this.habit_id = habit_id;
            this.user_id = user_id;
            this.name = name;
        }
    }
}
