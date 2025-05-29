using System.Data;
using System.Data.Common;
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

        public ITransaction StartTransaction()
        {
            return new StorageTransaction(_connectionString);
        }

        public IReadOnlyCollection<IReadOnlyDictionary<string, object>> ExecuteQueryInTransaction(
                string query,
                Dictionary<string, object> parameters,
                DbConnection connection,
                DbTransaction transaction
            )
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            using (DbCommand command = connection!.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = transaction;

                foreach (var param in parameters)
                {
                    command.Parameters.Add(new SQLiteParameter(param.Key, param.Value));
                }

                using (IDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }

                        result.Add(row);
                    }
                }
            }

            return result;
        }

        public uint ExecuteNonQueryInTransaction(
                string query,
                Dictionary<string, object> parameters,
                DbConnection connection,
                DbTransaction transaction
            )
        {

            uint rowsAffected;

            using (DbCommand command = connection!.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = transaction;

                foreach (var param in parameters)
                {
                    command.Parameters.Add(new SQLiteParameter(param.Key, param.Value));
                }

                rowsAffected = Convert.ToUInt32(command.ExecuteNonQuery());
            }

            return rowsAffected;
        }

        public IReadOnlyCollection<IReadOnlyDictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object> parameters)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

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
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

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
