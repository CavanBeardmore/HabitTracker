using System.Data.SQLite;

namespace HabitTracker.Server.Classes.User
{
    public interface IUserRepository
    {
        void OpenConnection(SQLiteConnection sqlite_conn);
        User? GetById(int id);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
    }
}
