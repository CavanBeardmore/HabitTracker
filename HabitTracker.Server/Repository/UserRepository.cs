using HabitTracker.Server.Models;
using HabitTracker.Server.Facade;
using HabitTracker.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using HabitTracker.Server.Database;
using HabitTracker.Server.Transformer;
using System.Data;

namespace HabitTracker.Server.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IStorage _sqliteFacade;
        private readonly IHabitTrackerDbContext _dbContext;
        private readonly ITransformer<User, IDataReader> _transformer;

        public UserRepository(IStorage sqliteFacade, IHabitTrackerDbContext dbContext, ITransformer<User, IDataReader> transformer)
        {
            _sqliteFacade = sqliteFacade;
            _dbContext = dbContext;
            _transformer = transformer;
        }

        public User? GetByUsername(string username)
        {
            string query = "SELECT * FROM Users WHERE Users.Username = @Username AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            IReadOnlyCollection<User> users = _sqliteFacade.ExecuteQuery<User>(
                query,
                _transformer.Transform,
                parameters
            );

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

        public bool Delete(string username)
        {
            string query = "UPDATE Users SET IsDeleted = 1 WHERE Username = @Username AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            uint rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Update(PatchUser user)
        {

            List<string> setClauses = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername }
            };

            if (!string.IsNullOrEmpty(user.NewUsername))
            {
                setClauses.Add("Username = @newUsername");
                parameters.Add("@newUsername", user.NewUsername);
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                setClauses.Add("Email = @email");
                parameters.Add("@email", user.Email);
            }

            if (!string.IsNullOrEmpty(user.NewPassword))
            {
                setClauses.Add("Password = @password");
                parameters.Add("@password", user.NewPassword);
            }

            string query = $"UPDATE Users SET {string.Join(", ", setClauses)} WHERE Username = @oldUsername AND IsDeleted = 0;";

            uint rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }
    }
}
