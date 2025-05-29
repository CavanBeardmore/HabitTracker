using HabitTracker.Server.Models;
using HabitTracker.Server.Storage;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Database;
using HabitTracker.Server.Transformer;

namespace HabitTracker.Server.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IStorage _sqliteFacade;
        private readonly ITransformer<IReadOnlyCollection<User>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> _transformer;

        public UserRepository(IStorage sqliteFacade, ITransformer<IReadOnlyCollection<User>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> transformer)
        {
            _sqliteFacade = sqliteFacade;
            _transformer = transformer;
        }

        public User? GetById(int userId)
        {
            string query = "SELECT * FROM Users WHERE Users.Id = @UserId AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _sqliteFacade.ExecuteQuery(
                query,
                parameters
            );

            IReadOnlyCollection<User> users = _transformer.Transform(result);

            return users.FirstOrDefault();
        }

        public User? GetByUsername(string username)
        {
            string query = "SELECT * FROM Users WHERE Users.Username = @Username AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _sqliteFacade.ExecuteQuery(
                query,
                parameters
            );

            IReadOnlyCollection<User> users = _transformer.Transform(result);

            return users.FirstOrDefault();
        }

        public bool Add(PostUser user)
        {
            string query = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Email", user.Email },
                { "@Password", user.Password }
            };

            uint rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Delete(int userId)
        {
            string query = "UPDATE Users SET IsDeleted = 1 WHERE Id = @UserId AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            uint rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public User? Update(int userId, PatchUser user)
        {

            List<string> setClauses = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@UserId", userId }
            };

            if (!string.IsNullOrWhiteSpace(user.NewUsername))
            {
                setClauses.Add("Username = @newUsername");
                parameters.Add("@newUsername", user.NewUsername);
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                setClauses.Add("Email = @email");
                parameters.Add("@email", user.Email);
            }

            if (!string.IsNullOrWhiteSpace(user.NewPassword))
            {
                setClauses.Add("Password = @password");
                parameters.Add("@password", user.NewPassword);
            }

            string query = $"UPDATE Users SET {string.Join(", ", setClauses)} WHERE Id = @UserId AND IsDeleted = 0;";

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> result = _sqliteFacade.ExecuteQuery(
                query,
                parameters
            );

            IReadOnlyCollection<User> users = _transformer.Transform(result);

            return users.FirstOrDefault();
        }
    }
}
