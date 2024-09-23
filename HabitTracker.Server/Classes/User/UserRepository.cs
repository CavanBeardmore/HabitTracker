using System.Data.SQLite;

namespace HabitTracker.Server.Classes.User
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
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

        public User? GetByUsername(string username)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "SELECT * FROM Users WHERE Users.username = @username;";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@username", username));

                    using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            string userName = (string)sqlite_datareader["username"];
                            string email = (string)sqlite_datareader["email"];
                            string password = (string)sqlite_datareader["password"];

                            return new User(userName, email, password);
                        }
                    }
                }
            }

            return null;
        }

        public void Add(User user)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "INSERT INTO Users (username, email, password) VALUES (@username, @email, @password);";

                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@username", user.username));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@email", user.email));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@password", user.password));

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(string username)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "DELETE FROM Users WHERE Users.username = @username;";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@username", username));

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(User user, string oldUsername)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "UPDATE Users SET username = @newUsername, email = @email, password = @password WHERE username = @oldUsername;";

                    sqlite_cmd.Parameters.AddWithValue("@newUsername", user.username);
                    sqlite_cmd.Parameters.AddWithValue("@oldUsername", oldUsername);
                    sqlite_cmd.Parameters.AddWithValue("@email", user.email);
                    sqlite_cmd.Parameters.AddWithValue("@password", user.password);

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
