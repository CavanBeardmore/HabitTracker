using HabitTracker.Server.Models;
using HabitTracker.Server.Facade;
using HabitTracker.Server.DTOs;


namespace HabitTracker.Server.Repository
{
    public class HabitRepository : IHabitRepository
    {
        private readonly ISqliteFacade _sqliteFacade;

        public HabitRepository(ISqliteFacade sqliteFacade)
        {
            _sqliteFacade = sqliteFacade;
        }

        public IEnumerable<Habit> GetAllByUserId(int user_id)
        {
            string query = "SELECT * FROM Habits WHERE Habits.User_id = @User_id";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", user_id }
            };

            return _sqliteFacade.ExecuteQuery<Habit>(
                query, 
                reader =>
                {
                    return new Habit
                    (
                        Convert.ToInt32(reader["Id"]),
                        Convert.ToInt32(reader["User_id"]),
                        (string)reader["name"]
                    );
                },
                parameters
            );
        }

        public Habit? GetById(int id)
        {
            string query = "SELECT * FROM Habits WHERE Habits.Id = @id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };
            
            List<Habit> habits = _sqliteFacade.ExecuteQuery<Habit>(
                query,
                reader =>
                {
                    return new Habit
                    (
                        Convert.ToInt32(reader["Id"]),
                        Convert.ToInt32(reader["User_id"]),
                        (string)reader["name"]
                    );
                },
                parameters
            );

            if ( habits.Count > 0)
            {
                return habits[0];
            }

            return null;
        }

        public bool Add(PostHabit habit)
        {
            string query = "INSERT INTO Habits (User_id, name) VALUES (@User_id, @name);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", habit.User_id },
                { "@name", habit.Name }
            };

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Delete(int id)
        {
            string query = "DELETE FROM Habits WHERE Habits.Id = @id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id },
            };

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Update(PatchHabit habit)
        {
            string query = "UPDATE Habits SET name = @name WHERE Id = @id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habit.Id },
                { "@name", habit.Name },
            };

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }
    }
}
