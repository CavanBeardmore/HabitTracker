using System.Data.SQLite;

namespace HabitTracker.Server.Classes.HabitLog
{
    public interface IHabitLogRepository
    {
        void OpenConnection(SQLiteConnection sqlite_conn);
        IEnumerable<HabitLog> GetAllByHabitId(int id);
        HabitLog? GetById(int id);
        void Add(HabitLog habitLog);
        void Update(HabitLog habitLog);
        void Delete(int id);
    }
}
