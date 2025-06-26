using HabitTracker.Server.Models;
using HabitTracker.Server.Storage;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Transformer;
using System.Data.Common;

namespace HabitTracker.Server.Repository
{
    public class HabitRepository : IHabitRepository
    {
        public IStorage SqliteFacade { get; }
        private readonly ITransformer<IReadOnlyCollection<Habit>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> _transformer;

        public HabitRepository(IStorage sqliteFacade, ITransformer<IReadOnlyCollection<Habit>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> transformer)
        {
            SqliteFacade = sqliteFacade;
            _transformer = transformer;
        }

        public IReadOnlyCollection<Habit> GetAllByUserId(int user_id)
        {
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u ON h.User_id = u.Id WHERE h.User_id = @User_id AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", user_id }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = SqliteFacade.ExecuteQuery(
                query,
                parameters
            );

            return _transformer.Transform(result);
        }

        public Habit? GetById(int habitId, int userId, DbConnection? connection, DbTransaction? transaction)
        {
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u On h.User_id WHERE h.Id = @id AND h.User_id = @userId AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            bool isTransaction = connection != null && transaction != null;

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result;
            
            if (isTransaction)
            {
                result = SqliteFacade.ExecuteQueryInTransaction(
                    query,
                    parameters,
                    connection!,
                    transaction!
                );
            } else {
                result = SqliteFacade.ExecuteQuery(
                    query,
                    parameters
                );
            }

            IReadOnlyCollection<Habit> habits = _transformer.Transform(result);

            return habits.FirstOrDefault();
        }

        public Habit? Add(int userId, PostHabit habit)
        {
            string query = "INSERT INTO Habits (User_id, name, StreakCount) VALUES (@User_id, @name, @StreakCount) RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", userId },
                { "@name", habit.Name },
                { "@StreakCount", 0 }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = SqliteFacade.ExecuteQuery(
                    query, 
                    parameters
                );

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

            uint rowsAffected = SqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public Habit? Update(int userId, PatchHabit habit, DbConnection? connection, DbTransaction? transaction)
        {
            string query = "UPDATE Habits SET name = @name, StreakCount = @streakCount WHERE Id = @id AND User_id = @userId RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habit.Id },
                { "@name", habit.Name },
                { "@userId", userId },
                { "@streakCount", habit.StreakCount },
            };

            bool isTransaction = connection != null && transaction != null;

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result;

            if (isTransaction)
            {
                Console.WriteLine("updating as transaction");
                result = SqliteFacade.ExecuteQueryInTransaction(query, parameters, connection!, transaction!);
            } else
            {
                result = SqliteFacade.ExecuteQuery(query, parameters);
            }

            IReadOnlyCollection<Habit> habits = _transformer.Transform(result);

            return habits.FirstOrDefault();
        }
    }
}
