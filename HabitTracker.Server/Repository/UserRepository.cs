using HabitTracker.Server.Models;
using HabitTracker.Server.Facade;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ISqliteFacade _sqliteFacade;

        public UserRepository(ISqliteFacade sqliteFacade)
        {
            _sqliteFacade = sqliteFacade;
        }

        public User? GetByUsername(string username)
        {
            string query = "SELECT * FROM Users WHERE Users.Username = @Username;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            List<User> users = _sqliteFacade.ExecuteQuery<User>(
                query,
                reader =>
                {
                    return new User
                    (
                        Convert.ToInt32(reader["Id"]),
                        (string)reader["Username"],
                        (string)reader["Email"],
                        (string)reader["Password"]
                    );
                },
                parameters
            );

            if (users.Count > 0)
            {
                return users[0];
            }

            return null;
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

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Delete(string username)
        {
            string query = "DELETE FROM Users WHERE Users.Username = @Username;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

        public bool Update(PatchUser user)
        {
            string query = "UPDATE Users SET Username = @newUsername, Email = @email, Password = @password WHERE Username = @oldUsername;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
                { "@password", user.Password },
                { "@oldUsername", user.OldUsername },
            };

            int rowsAffected = _sqliteFacade.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }
    }
}
