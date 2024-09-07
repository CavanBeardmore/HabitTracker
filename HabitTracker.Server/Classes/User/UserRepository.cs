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

        public User? GetById(int id)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "SELECT * FROM Users WHERE Users.user_id = @id;";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@id", id));

                    using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            int user_id = (int)sqlite_datareader["user_id"];
                            string username = (string)sqlite_datareader["username"];
                            string email = (string)sqlite_datareader["email"];

                            return new User(user_id, username, email);
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
                    sqlite_cmd.CommandText = "INSERT INTO Users (user_id, username, email, password) VALUES (@user_id, @username, @email, @password);";

                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@user_id", user.user_id));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@username", user.username));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@email", user.email));
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@password", user.password));

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "DELETE FROM Users WHERE Users.user_id = @id;";
                    sqlite_cmd.Parameters.Add(new SQLiteParameter("@id", id));

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(User user)
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(_connectionString))
            {

                OpenConnection(sqlite_conn);

                using (SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = "UPDATE Users SET username = @username, email = @email, password = @password WHERE user_id = @user_id;";

                    sqlite_cmd.Parameters.AddWithValue("@user_id", user.user_id);
                    sqlite_cmd.Parameters.AddWithValue("@username", user.username);
                    sqlite_cmd.Parameters.AddWithValue("@email", user.email);
                    sqlite_cmd.Parameters.AddWithValue("@password", user.password);

                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
