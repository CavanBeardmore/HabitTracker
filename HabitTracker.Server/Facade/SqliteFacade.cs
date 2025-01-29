using System.Data;
using System.Data.SQLite;

namespace HabitTracker.Server.Facade
{
    public class SqliteFacade : IStorage
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

        public IReadOnlyCollection<T> ExecuteQuery<T>(string query, Func<IDataReader, T> transform, Dictionary<string, object> parameters)
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

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        List<T> result = new List<T>();
                        while (reader.Read())
                        {
                            result.Add(transform(reader));
                        }
                        return result;
                    }
                }
            }
        }

        public uint ExecuteNonQuery(string query, Dictionary<string, object> parameters)
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

                    return Convert.ToUInt32(command.ExecuteNonQuery());
                }
            }
        }
    }
}
