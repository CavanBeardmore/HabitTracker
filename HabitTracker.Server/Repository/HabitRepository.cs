using HabitTracker.Server.Models;
using HabitTracker.Server.Facade;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Transformer;
using System.Data;


namespace HabitTracker.Server.Repository
{
    public class HabitRepository : IHabitRepository
    {
        private readonly IStorage _sqliteFacade;
        private readonly ITransformer<Habit, IDataReader> _transformer;

        public HabitRepository(IStorage sqliteFacade, ITransformer<Habit, IDataReader> transformer)
        {
            _sqliteFacade = sqliteFacade;
            _transformer = transformer;
        }

        public IReadOnlyCollection<Habit> GetAllByUserId(int user_id)
        {
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u ON h.User_id = u.Id WHERE h.User_id = @User_id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", user_id }
            };

            return _sqliteFacade.ExecuteQuery<Habit>(
                query, 
                _transformer.Transform,
                parameters
            );
        }

        public Habit? GetById(int habitId, int userId)
        {
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u On h.User_id WHERE h.Id = @id AND h.User_id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };
            
            IReadOnlyCollection<Habit> habits = _sqliteFacade.ExecuteQuery<Habit>(
                query,
                _transformer.Transform,
                parameters
            );

            return habits.FirstOrDefault();
        }

        public bool Add(PostHabit habit)
        {
            string query = "INSERT INTO Habits (User_id, name) VALUES (@User_id, @name);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", habit.User_id },
                { "@name", habit.Name }
            };

            uint rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Delete(int habitId, int userId)
        {
            string query = "DELETE FROM Habits WHERE Habits.Id = @id AND Habits.User_id = @userId;";
             
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            uint rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Update(PatchHabit habit)
        {
            string query = "UPDATE Habits SET name = @name WHERE Id = @id AND User_id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habit.Id },
                { "@name", habit.Name },
                { "@userId", habit.UserId },
            };

            uint rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }
    }
}
