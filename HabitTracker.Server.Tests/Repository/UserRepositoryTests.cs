using HabitTracker.Server.Database;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Facade;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Transformer;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Server.Tests.Repository
{
    public class UserRepositoryTests
    {

        private readonly UserRepository _repository;
        private readonly Mock<IHabitTrackerDbContext> _mockDbContext;
        private readonly Mock<IStorage> _mockFacade;
        private readonly Mock<ITransformer<User, IDataReader>> _mockTransformer;

        public UserRepositoryTests()
        {
            _mockDbContext = new Mock<IHabitTrackerDbContext>();
            _mockFacade = new Mock<IStorage>();
            _mockTransformer = new Mock<ITransformer<User, IDataReader>>();
            _repository = new UserRepository(_mockFacade.Object, _mockDbContext.Object, _mockTransformer.Object);
        }

        [Fact]
        public void GetByUsername_ReturnsUser()
        {
            string username = "test";

            string query = "SELECT * FROM Users WHERE Users.Username = @Username;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(new List<User> { new User(1234, "test", "test1", "test2") });

            var result = _repository.GetByUsername(username);

            Assert.NotNull(result);
            Assert.Equal(1234,result.Id);
            Assert.Equal(username, result.Username);
            Assert.Equal("test1", result.Email);
            Assert.Equal("test2", result.Password);
        }

        [Fact]
        public void GetByUsername_ReturnsNull()
        {
            string username = "test";

            string query = "SELECT * FROM Users WHERE Users.Username = @Username;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(new List<User>());

            var result = _repository.GetByUsername(username);

            Assert.Null(result);
        }
    }
}
