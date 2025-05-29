using System.Data.Common;

namespace HabitTracker.Server.Storage
{
    public interface ITransaction
    {
        DbConnection Connection { get; }
        DbTransaction Transaction { get; }
        void Commit();
        void Rollback();
        void Dispose();
    }
}
