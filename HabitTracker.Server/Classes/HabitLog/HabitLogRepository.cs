using System.Data.SQLite;

namespace HabitTracker.Server.Classes.HabitLog
{
    public class HabitLogRepository : IHabitLogRepository
    {
        private readonly string _connectionString;

        public HabitLogRepository(string connectionString)
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

        public void CloseConnection(SQLiteConnection sqlite_conn)
        {
            try
            {
                sqlite_conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing connection" + ex.Message);
            }
        }

        public IEnumerable<HabitLog> GetAllByHabitId(int id)
        {
            List<HabitLog> habitLogs = new List<HabitLog>();
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {
                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "SELECT * FROM HabitLog WHERE HabitLog.habit_id = @id";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@id", id));

                    using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            int habit_id = (int)sqlite_datareader["habit_id"];
                            int habitlog_id = (int)sqlite_datareader["habitlog_id"];
                            DateTime start_date = (DateTime)sqlite_datareader["start_date"];
                            bool habit_logged = (bool)sqlite_datareader["habit_met"];
                            int period_type = (int)sqlite_datareader["period_type"];

                            habitLogs.Add(new HabitLog(habit_id, habitlog_id, start_date, habit_logged, period_type));
                        }
                    }
                }

                return habitLogs;
            }
        }

        public HabitLog? GetById(int id)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "SELECT * FROM HabitLog WHERE HabitLog.habitlog_id = @id";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@id", id));

                    using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            int habit_id = (int)sqlite_datareader["habit_id"];
                            int habitlog_id = (int)sqlite_datareader["habitlog_id"];
                            DateTime start_date = (DateTime)sqlite_datareader["start_date"];
                            bool habit_logged = (bool)sqlite_datareader["habit_met"];
                            int period_type = (int)sqlite_datareader["period_type"];

                            return new HabitLog(habitlog_id, habit_id, start_date, habit_logged, period_type);
                        }
                    }
                }
            }

            return null;
        }

        public void Add(HabitLog habitLog)
        {
            int habitlog_id = habitLog.habitLog_id;
            int habit_id = habitLog.habit_id;
            DateTime start_date = habitLog.start_date;
            bool habit_logged = habitLog.habit_logged;
            int period_type = habitLog.period_type;

            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "INSERT INTO HabitLog (habitlog_id, habit_id, start_date, habit_met, period_type) VALUES (@habitlog_id, @habit_id, @start_date, @habit_met, @period_type);";

                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@habitlog_id", habitlog_id));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@habit_id", habit_id));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@start_date", start_date));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@habit_met", habit_logged));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@period_type", period_type));

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(HabitLog habitLog)
        {
            try
            {
                using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
                {

                    OpenConnection(sqlite_conn);

                    using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                    {
                        sqlite_cmd.CommandText = "UPDATE HabitLog SET habit_met = @habit_met WHERE habitlog_id = @habitlog_id;";

                        sqlite_cmd.Parameters.AddWithValue("@habitlog_id", habitLog.habitLog_id);
                        sqlite_cmd.Parameters.AddWithValue("@habit_met", habitLog.habit_logged);

                        sqlite_cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public void Delete(int id)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "DELETE FROM HabitLog WHERE Habitlog.habitlog_id = @id;";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@id", id));

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
