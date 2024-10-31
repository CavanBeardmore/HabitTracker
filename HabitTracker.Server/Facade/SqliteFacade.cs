using System.Data.SQLite;

namespace HabitTracker.Server.Facade
{
    public class SqliteFacade : ISqliteFacade
    {
        private readonly string _connectionString;

        public SqliteFacade(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        private void OpenConnection(SQLiteConnection connection)
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening connection" + ex.Message);
            }
        }

        public List<T> ExecuteQuery<T>(string query, Func<SQLiteDataReader, T> map, Dictionary<string, object> parameters)
        {
            using (SQLiteConnection connection = CreateConnection())
            {
                OpenConnection(connection);

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(new SQLiteParameter(param.Key, param.Value));
                    }

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        List<T> result = new List<T>();
                        while (reader.Read())
                        {
                            result.Add(map(reader));
                        }
                        return result;
                    }
                }
            }
        }

        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            using (SQLiteConnection connection = CreateConnection())
            {
                OpenConnection(connection);

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(new SQLiteParameter(param.Key, param.Value));
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}
