using System.Data;

namespace HabitTracker.Server.Facade
{
    public interface IStorage
    {
        IReadOnlyCollection<T> ExecuteQuery<T>(string query, Func<IDataReader, T> transform, Dictionary<string, object> parameters);
        uint ExecuteNonQuery(string query, Dictionary<string, object> parameters);
    }
}
