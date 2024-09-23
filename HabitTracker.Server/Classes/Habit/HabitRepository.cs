using System.Data.SQLite;
using System.Xml.Linq;

namespace HabitTracker.Server.Classes.Habit
{
    public class HabitRepository : IHabitRepository
    {
        private readonly string _connectionString;

        public HabitRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void OpenConnection(SQLiteConnection sqlite_conn)
        {
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening connection" + ex.Message);
            }
        }

        public IEnumerable<Habit> GetAllByUsername(string username)
        {
            List<Habit> habits = new List<Habit>();
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            { 
                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "SELECT * FROM Habits WHERE Habits.username = @username";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@username", username));

                    using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            int habit_id = (int)sqlite_datareader["habit_id"];
                            string userName = (string)sqlite_datareader["username"];
                            string name = (string)sqlite_datareader["name"];

                            habits.Add(new Habit(habit_id, userName, name));
                        }
                    }
                }

                return habits;
            }
        }

        public Habit? GetById(int id)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "SELECT * FROM Habits WHERE Habits.habit_id = @id;";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@id", id));

                    using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            int habit_id = (int)sqlite_datareader["habit_id"];
                            string username = (string)sqlite_datareader["username"];
                            string name = (string)sqlite_datareader["name"];

                            return new Habit(habit_id, username, name);
                        }
                    }
                }
            }

            return null;
        }

        public void Add(Habit habit)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "INSERT INTO Habits (habit_id, username, name) VALUES (@habit_id, @username, @name);";

                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@habit_id", habit.habit_id));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@username", habit.username));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@name", habit.name));

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "DELETE FROM Habits WHERE Habits.habit_id = @id;";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@id", id));

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Habit habit)
        {

            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "UPDATE Habits SET name = @name WHERE habit_id = @habit_id;";

                    sqlite_cmd.Parameters.AddWithValue("@habit_id", habit.habit_id);
                    sqlite_cmd.Parameters.AddWithValue("@name", habit.name);

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
