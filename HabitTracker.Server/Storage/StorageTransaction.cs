using System.Data.SQLite;
using System.Data.Common;

namespace HabitTracker.Server.Storage
{
    public class StorageTransaction : ITransaction
    {
        public DbConnection Connection { get; }
        public DbTransaction Transaction { get; }
        public StorageTransaction(string connectionString) 
        {
            Connection = new SQLiteConnection(connectionString);
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }

        public void Dispose()
        {
            Transaction.Dispose();
            Connection.Dispose();
        }
    }
}
