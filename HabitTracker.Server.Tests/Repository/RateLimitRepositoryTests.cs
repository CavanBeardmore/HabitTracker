using HabitTracker.Server.Storage;
using Moq;
using HabitTracker.Server.Transformer;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Repository;

namespace HabitTracker.Server.Tests.Repository
{
    public class RateLimitRepositoryTests
    {
        private readonly Mock<IStorage> _mockStorage;
        private readonly Mock<ITransformer<IReadOnlyCollection<Rate>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>> _mockTransformer;
        private readonly IRateLimitRepository _rateLimitRepository;

        public RateLimitRepositoryTests()
        {
            _mockStorage = new Mock<IStorage>();
            _mockTransformer = new Mock<ITransformer<IReadOnlyCollection<Rate>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>>();
            _rateLimitRepository = new RateLimitRepository(_mockStorage.Object, _mockTransformer.Object);
        }

        [Fact]
        public void GetRate_ReturnsRate()
        {
            string ipAddress = "127.0.0.1";
            DateTime date = DateTime.UtcNow;
            string query = "SELECT * FROM Rates Where IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ipAddress", ipAddress }
            };

            List<Dictionary<string, object>> facadeData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "IpAddress", ipAddress },
                    { "Count", 1 },
                    { "Ttl", date }
                }
            };

            List<Rate> transformerData = new List<Rate>
            {
                new Rate(ipAddress, 1, date)
            };

            _mockStorage.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _rateLimitRepository.GetRate(ipAddress);

            Assert.NotNull(result);
            Assert.True(result.IpAddress == ipAddress);
            Assert.True(result.Count == 1);
            Assert.True(result.Ttl == date);
        }

        [Fact]
        public void GetRate_ReturnsNull()
        {
            string ipAddress = "127.0.0.1";
            DateTime date = DateTime.UtcNow;
            string query = "SELECT * FROM Rates Where IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ipAddress", ipAddress }
            };

            List<Dictionary<string, object>> facadeData = new List<Dictionary<string, object>>();
            List<Rate> transformerData = new List<Rate>();

            _mockStorage.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _rateLimitRepository.GetRate(ipAddress);

            Assert.Null(result);
        }

        [Fact]
        public void AddRate_ReturnsRate()
        {
            string ipAddress = "127.0.0.1";
            uint count = 1;
            DateTime date = DateTime.UtcNow;
            string query = "INSERT INTO Rates (IpAddress, Count, Ttl) VALUES (@ipAddress, @count, @ttl) RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ipAddress", ipAddress },
                { "@count", count },
                { "@ttl", date }
            };

            List<Dictionary<string, object>> facadeData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "IpAddress", ipAddress },
                    { "Count", count },
                    { "Ttl", date }
                }
            };

            List<Rate> transformerData = new List<Rate>
            {
                new Rate(ipAddress, count, date)
            };

            _mockStorage.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _rateLimitRepository.AddRate(count, ipAddress, date);

            Assert.NotNull(result);
            Assert.True(result.IpAddress == ipAddress);
            Assert.True(result.Count == 1);
            Assert.True(result.Ttl == date);
        }

        [Fact]
        public void AddRate_ReturnsNull()
        {
            string ipAddress = "127.0.0.1";
            uint count = 1;
            DateTime date = DateTime.UtcNow;
            string query = "INSERT INTO Rates (IpAddress, Count, Ttl) VALUES (@ipAddress, @count, @ttl) RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ipAddress", ipAddress },
                { "@count", count },
                { "@ttl", date }
            };

            List<Dictionary<string, object>> facadeData = new List<Dictionary<string, object>>();
            List<Rate> transformerData = new List<Rate>();

            _mockStorage.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _rateLimitRepository.AddRate(count, ipAddress, date);

            Assert.Null(result);
        }

        [Fact]
        public void UpdateRateCount_ReturnsTrue()
        {
            uint count = 1;
            string ipAddress = "127.0.0.1";

            string query = "UPDATE Rates SET Count = @count WHERE IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@count", count },
                { "@ipAddress", ipAddress }
            };

            _mockStorage.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _rateLimitRepository.UpdateRateCount(count, ipAddress);

            Assert.True(result);
        }

        [Fact]
        public void UpdateRateCount_ReturnsFalse()
        {
            uint count = 1;
            string ipAddress = "127.0.0.1";

            string query = "UPDATE Rates SET Count = @count WHERE IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "count", count },
                { "ipAddress", ipAddress }
            };

            _mockStorage.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _rateLimitRepository.UpdateRateCount(count, ipAddress);

            Assert.False(result);
        }

        [Fact]
        public void UpdateRateCountAndTtl_ReturnsTrue()
        {
            uint count = 1;
            string ipAddress = "127.0.0.1";
            DateTime date = DateTime.UtcNow;

            string query = "UPDATE Rates SET Count = @count, Ttl = @ttl WHERE IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@count", count },
                { "@ipAddress", ipAddress },
                { "@ttl", date }
            };

            _mockStorage.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _rateLimitRepository.UpdateRateCountAndTtl(count, ipAddress, date);

            Assert.True(result);
        }

        [Fact]
        public void UpdateRateCountAndTtl_ReturnsFalse()
        {
            uint count = 1;
            string ipAddress = "127.0.0.1";
            DateTime date = DateTime.UtcNow;

            string query = "UPDATE Rates SET Count = @count, Ttl = @ttl WHERE IpAddress = @ipAddress";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "count", count },
                { "ipAddress", ipAddress },
                { "ttl", date }
            };

            _mockStorage.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _rateLimitRepository.UpdateRateCountAndTtl(count, ipAddress, date);

            Assert.False(result);
        }
    }
}
