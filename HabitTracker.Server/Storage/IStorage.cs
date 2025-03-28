using System.Data;

namespace HabitTracker.Server.Storage
{
    public interface IStorage
    {
        IReadOnlyCollection<IReadOnlyDictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object> parameters);
        uint ExecuteNonQuery(string query, Dictionary<string, object> parameters);
    }
}
