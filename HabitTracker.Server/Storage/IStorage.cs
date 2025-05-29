using System.Data.Common;
using System.Data.SQLite;

namespace HabitTracker.Server.Storage
{
    public interface IStorage
    {
        IReadOnlyCollection<IReadOnlyDictionary<string, object>> ExecuteQueryInTransaction(
            string query, 
            Dictionary<string, object> parameters,
            DbConnection connection,
            DbTransaction transaction
        );
        uint ExecuteNonQueryInTransaction(
            string query, 
            Dictionary<string, object> parameters,
            DbConnection connection,
            DbTransaction transaction
        );

        IReadOnlyCollection<IReadOnlyDictionary<string, object>> ExecuteQuery(
            string query,
            Dictionary<string, object> parameters
        );
        uint ExecuteNonQuery(
            string query,
            Dictionary<string, object> parameters
        );
        ITransaction StartTransaction();
    }
}
