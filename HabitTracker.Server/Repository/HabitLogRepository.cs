using HabitTracker.Server.Models;
using HabitTracker.Server.Storage;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Transformer;
using System.Data.Common;

namespace HabitTracker.Server.Repository
{
    public class HabitLogRepository : IHabitLogRepository
    {
        private readonly IStorage _storage;
        private readonly ITransformer<IReadOnlyCollection<HabitLog>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> _transformer;

        public HabitLogRepository(IStorage storage, ITransformer<IReadOnlyCollection<HabitLog>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> transformer)
        {
            _storage = storage;
            _transformer = transformer;
        }

        public IReadOnlyCollection<HabitLog> GetAllByHabitId(int id, int userId, int pageNumber)
        {
            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Habit_id = @id AND u.Id = @userId AND u.IsDeleted = 0 ORDER BY Start_date DESC LIMIT 30 OFFSET @offset;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id },
                { "@userId", userId },
                {"@offset", pageNumber }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _storage.ExecuteQuery(
                query,
                parameters
            );

            return _transformer.Transform(result);
        }

        public HabitLog? GetById(int habitLogId, int userId)
        {
            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Id = @id AND u.Id = @userId AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitLogId },
                { "@userId", userId }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _storage.ExecuteQuery(
                query,
                parameters
            );

            IReadOnlyCollection<HabitLog> habitLogs = _transformer.Transform(result);

            return habitLogs.FirstOrDefault();
        }

        public HabitLog? GetByHabitIdAndStartDate(int habitId, int userId, DateTime date)
        {
            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Habit_id = @habitId AND hl.Start_date = @date AND u.Id = @userId AND u.IsDeleted = 0"; ;

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@habitId", habitId },
                { "@userId", userId },
                { "@date", date }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _storage.ExecuteQuery(
                query,
                parameters
            );

            IReadOnlyCollection<HabitLog> habitLogs = _transformer.Transform(result);

            return habitLogs.FirstOrDefault();
        }

        public HabitLog? GetMostRecentHabitLog(int habitId, int userId, DbConnection connection, DbTransaction transaction)
        {
            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE u.IsDeleted = 0 ORDER BY Start_date DESC LIMIT 1";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _storage.ExecuteQueryInTransaction(
                query,
                parameters,
                connection,
                transaction
            );

            IReadOnlyCollection<HabitLog> habitLogs = _transformer.Transform(result);

            return habitLogs.FirstOrDefault();
        }

        public HabitLog? Add(PostHabitLog habitLog, DbConnection connection, DbTransaction transaction)
        {
            string query = "INSERT INTO HabitLogs (Habit_id, Start_date, Habit_logged, Length_in_days) VALUES (@Habit_id, @Start_date, @Habit_logged, @Length_in_days) RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Habit_id", habitLog.Habit_id },
                { "@Start_date", habitLog.Start_date },
                { "@Habit_logged", habitLog.Habit_logged },
                { "@Length_in_days", habitLog.Length_in_days }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _storage.ExecuteQueryInTransaction(query, parameters, connection, transaction);

            IReadOnlyCollection<HabitLog?> habitLogs = _transformer.Transform(result);

            return habitLogs.FirstOrDefault();
        }

        public HabitLog? Update(PatchHabitLog habitLog)
        {
            string query = "UPDATE HabitLogs SET Habit_logged = @Habit_logged WHERE Id = @Id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Id", habitLog.Id },
                { "@Habit_logged", habitLog.Habit_logged },
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _storage.ExecuteQuery(query, parameters);

            IReadOnlyCollection<HabitLog?> habitLogs = _transformer.Transform(result);

            return habitLogs.FirstOrDefault();
        }

        public bool Delete(int habitLogId, int userId)
        {
            string query = "DELETE FROM HabitLogs WHERE Id = @id AND Habit_id IN (SELECT h.Id FROM Habits h INNER JOIN Users u ON h.User_id = u.Id WHERE u.Id = @userId);";
    
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitLogId },
                { "@userId", userId },
            };
            uint rowsAffected = _storage.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool DeleteByHabitIdAndStartDate(int habitId, DateTime startDate)
        {
            string query = "DELETE FROM HabitLogs WHERE HabitLogs.Habit_id = @habitId AND HabitLogs.Start_date = @startDate;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@habitId", habitId },
                { "@startDate", startDate },
            };

            uint rowsAffected = _storage.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }
    }
}
