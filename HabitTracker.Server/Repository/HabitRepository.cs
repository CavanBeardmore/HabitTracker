using HabitTracker.Server.Models;
using HabitTracker.Server.Storage;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Transformer;
using System.Data;


namespace HabitTracker.Server.Repository
{
    public class HabitRepository : IHabitRepository
    {
        private readonly IStorage _sqliteFacade;
        private readonly ITransformer<IReadOnlyCollection<Habit>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> _transformer;

        public HabitRepository(IStorage sqliteFacade, ITransformer<IReadOnlyCollection<Habit>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> transformer)
        {
            _sqliteFacade = sqliteFacade;
            _transformer = transformer;
        }

        public IReadOnlyCollection<Habit> GetAllByUserId(int user_id)
        {
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u ON h.User_id = u.Id WHERE h.User_id = @User_id AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", user_id }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _sqliteFacade.ExecuteQuery(
                query,
                parameters
            );

            return _transformer.Transform(result);
        }

        public Habit? GetById(int habitId, int userId)
        {
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u On h.User_id WHERE h.Id = @id AND h.User_id = @userId AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _sqliteFacade.ExecuteQuery(
                query,
                parameters
            );

            IReadOnlyCollection<Habit> habits = _transformer.Transform(result);

            return habits.FirstOrDefault();
        }

        public Habit? Add(int userId, PostHabit habit)
        {
            string query = "INSERT INTO Habits (User_id, name) VALUES (@User_id, @name) RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", userId },
                { "@name", habit.Name }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _sqliteFacade.ExecuteQuery(query, parameters);

            IReadOnlyCollection<Habit> habits = _transformer.Transform(result);

            return habits.FirstOrDefault();
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

        public bool Update(int userId, PatchHabit habit)
        {
            string query = "UPDATE Habits SET name = @name WHERE Id = @id AND User_id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habit.Id },
                { "@name", habit.Name },
                { "@userId", userId },
            };

            uint rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }
    }
}
