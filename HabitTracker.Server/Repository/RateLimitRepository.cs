using HabitTracker.Server.DTOs;
using HabitTracker.Server.Storage;
using HabitTracker.Server.Transformer;

namespace HabitTracker.Server.Repository
{
    public class RateLimitRepository : IRateLimitRepository
    {
        private readonly IStorage _database;
        private readonly ITransformer<IReadOnlyCollection<Rate>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> _transformer;

        public RateLimitRepository(IStorage database, ITransformer<IReadOnlyCollection<Rate>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>> transformer)
        {
            _database = database;
            _transformer = transformer;
        }

        public Rate? GetRate(string ipAddress)
        {
            string query = "SELECT * FROM Rates Where IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ipAddress", ipAddress }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> data = _database.ExecuteQuery(query, parameters);

            IReadOnlyCollection<Rate> rates = _transformer.Transform(data);

            return rates.FirstOrDefault();
        }

        public Rate? AddRate(uint count, string ipAddress, DateTime ttl)
        {
            string query = "INSERT INTO Rates (IpAddress, Count, Ttl) VALUES (@ipAddress, @count, @ttl) RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ipAddress", ipAddress },
                { "@count", count },
                { "@ttl", ttl }
            };

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> data = _database.ExecuteQuery(query, parameters);

            IReadOnlyCollection<Rate> rates = _transformer.Transform(data);

            return rates.FirstOrDefault();
        }

        public bool UpdateRateCount(uint count, string ipAddress)
        {
            string query = "UPDATE Rates SET Count = @count WHERE IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "count", count },
                { "ipAddress", ipAddress }
            };

            uint recordsUpdated = _database.ExecuteNonQuery(query, parameters);

            return recordsUpdated > 0;
        }

        public bool UpdateRateCountAndTtl(uint count, string ipAddress, DateTime date)
        {
            string query = "UPDATE Rates SET Count = @count, Ttl = @ttl WHERE IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "count", count },
                { "ipAddress", ipAddress },
                { "ttl", date }
            };

            uint recordsUpdated = _database.ExecuteNonQuery(query, parameters);

            return recordsUpdated > 0;
        }
    }
}
