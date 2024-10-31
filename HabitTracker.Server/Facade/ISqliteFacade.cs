using System.Data.SQLite;

namespace HabitTracker.Server.Facade
{
    public interface ISqliteFacade
    {
        List<T> ExecuteQuery<T>(string query, Func<SQLiteDataReader, T> map, Dictionary<string, object> parameters);
        int ExecuteNonQuery(string query, Dictionary<string, object> parameters);
    }
}
