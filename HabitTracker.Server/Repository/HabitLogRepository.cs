using HabitTracker.Server.Models;
using HabitTracker.Server.Facade;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Repository
{
    public class HabitLogRepository : IHabitLogRepository
    {
        private readonly ISqliteFacade _sqliteFacade;

        public HabitLogRepository(ISqliteFacade sqliteFacade)
        {
            _sqliteFacade = sqliteFacade;
        }

        public IEnumerable<HabitLog> GetAllByHabitId(int id)
        {
            string query = "SELECT * FROM HabitLogs WHERE HabitLogs.Habit_id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            return _sqliteFacade.ExecuteQuery<HabitLog>(
                query,
                reader =>
                {
                    return new HabitLog
                    (
                        Convert.ToInt32(reader["Id"]),
                        Convert.ToInt32(reader["Habit_id"]),
                        Convert.ToDateTime(reader["Start_date"]),
                        Convert.ToBoolean(reader["Habit_logged"]),
                        Convert.ToInt32(reader["Length_in_days"])
                    );
                },
                parameters
            );
        }

        public HabitLog? GetById(int id)
        {
            string query = "SELECT * FROM HabitLogs WHERE HabitLogs.Id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            List<HabitLog> habitLogs = _sqliteFacade.ExecuteQuery<HabitLog>(
                query,
                reader =>
                {
                    return new HabitLog
                    (
                        Convert.ToInt32(reader["Id"]),
                        Convert.ToInt32(reader["Habit_id"]),
                        Convert.ToDateTime(reader["Start_date"]),
                        Convert.ToBoolean(reader["Habit_logged"]),
                        Convert.ToInt32(reader["Length_in_days"])
                    );
                },
                parameters
            );

            if (habitLogs.Count > 0)
            {
                return habitLogs[0];
            }

            return null;
        }

        public bool Add(PostHabitLog habitLog)
        {
            string query = "INSERT INTO HabitLogs (Habit_id, Start_date, Habit_logged, Length_in_days) VALUES (@Habit_id, @Start_date, @Habit_logged, @Length_in_days);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Habit_id", habitLog.Habit_id },
                { "@Start_date", habitLog.Start_date },
                { "@Habit_logged", habitLog.Habit_logged },
                { "@Length_in_days", habitLog.Length_in_days }
            };

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Update(PatchHabitLog habitLog)
        {
            string query = "UPDATE HabitLogs SET Habit_logged = @Habit_logged WHERE Id = @Id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Id", habitLog.Id },
                { "@Habit_logged", habitLog.Habit_logged },
            };

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string query = "DELETE FROM HabitLogs WHERE HabitLogs.Id = @id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id },
            };

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }
    }
}
