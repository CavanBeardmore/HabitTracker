using System.Data.SQLite;

namespace HabitTracker.Server.Classes.User
{
    public interface IUserRepository
    {
        void OpenConnection(SQLiteConnection sqlite_conn);
        User? GetByUsername(string username);
        void Add(User user);
        void Update(User user, string oldUsername);
        void Delete(string username);
    }
}
