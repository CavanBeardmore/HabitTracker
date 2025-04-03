using System.Data;
using System.Data.SQLite;

namespace HabitTracker.Server.Storage
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

        public IReadOnlyCollection<IReadOnlyDictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object> parameters)
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
                        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }

                            result.Add(row);
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
