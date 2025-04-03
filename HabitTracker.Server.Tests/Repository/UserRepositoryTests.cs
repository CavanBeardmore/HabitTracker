using HabitTracker.Server.Database;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Storage;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Transformer;
using Moq;

namespace HabitTracker.Server.Tests.Repository
{
    public class UserRepositoryTests
    {

        private readonly UserRepository _repository;
        private readonly Mock<IStorage> _mockFacade;
        private readonly Mock<ITransformer<IReadOnlyCollection<User>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>> _mockTransformer;

        public UserRepositoryTests()
        {
            _mockFacade = new Mock<IStorage>();
            _mockTransformer = new Mock<ITransformer<IReadOnlyCollection<User>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>>();
            _repository = new UserRepository(_mockFacade.Object, _mockTransformer.Object);
        }

        [Fact]
        public void GetById_ReturnsUser()
        {
            int userId = 1234;

            string query = "SELECT * FROM Users WHERE Users.Id = @UserId AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<User> transformerData = new List<User> { new User(userId, "test", "test2", "test3") };

            facadeData.Add(new Dictionary<string, object>{
                { "Id", userId },
                { "Username", "test" },
                { "Email", "test2" },
                { "Password", "test3" }
            });

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetById(userId);

            Assert.NotNull(result);
            Assert.Equal(1234, result.Id);
            Assert.Equal("test", result.Username);
            Assert.Equal("test2", result.Email);
            Assert.Equal("test3", result.Password);
        }

        [Fact]
        public void GetById_ReturnsNull()
        {
            int userId = 1234;

            string query = "SELECT * FROM Users WHERE Users.Id = @UserId AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<User> transformerData = new List<User>();

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetById(userId);

            Assert.Null(result);
        }

        [Fact]
        public void GetByUsername_ReturnsUser()
        {
            string username = "test";

            string query = "SELECT * FROM Users WHERE Users.Username = @Username AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<User> transformerData = new List<User> { new User(1234, "test", "test2", "test3") };

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "Username", "test" },
                { "Email", "test2" },
                { "Password", "test3" }
            });


            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetByUsername(username);

            Assert.NotNull(result);
            Assert.Equal(1234, result.Id);
            Assert.Equal(username, result.Username);
            Assert.Equal("test2", result.Email);
            Assert.Equal("test3", result.Password);
        }

        [Fact]
        public void GetByUsername_ReturnsNull()
        {
            string username = "test";

            string query = "SELECT * FROM Users WHERE Users.Username = @Username AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };


            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<User> transformerData = new List<User>();

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetByUsername(username);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ReturnsTrue()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            string query = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Email", user.Email },
                { "@Password", user.Password }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Add(user);

            Assert.True(result);
        }

        [Fact]
        public void Add_ReturnsFalse()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            string query = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Email", user.Email },
                { "@Password", user.Password }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Add(user);

            Assert.False(result);
        }

        [Fact]
        public void Delete_ReturnsTrue()
        {
            int userId = 1234;

            string query = "UPDATE Users SET IsDeleted = 1 WHERE Id = @UserId AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Delete(1234);

            Assert.True(result);
        }

        [Fact]
        public void Delete_ReturnsFalse()
        {
            int userId = 1234;

            string query = "UPDATE Users SET IsDeleted = 1 WHERE Id = @UserId AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Delete(1234);

            Assert.False(result);
        }

        [Fact]
        public void Update_WithAllParamsReturnTrue()
        {
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@UserId", 1234 },
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Username = @newUsername, Email = @email, Password = @password WHERE Id = @UserId AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(1234, user);

            Assert.True(result);
        }

        [Fact]
        public void Update_WithAllParamsReturnFalse()
        {
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@UserId", 1234 },
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Username = @newUsername, Email = @email, Password = @password WHERE Id = @UserId AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(1234, user);

            Assert.False(result);
        }

        [Fact]
        public void Update_WithoutNewUsernameParamsReturnTrue()
        {
            PatchUser user = new PatchUser(oldPassword: "test2", email: "test3", newPassword: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@UserId", 1234 },
                { "@email", user.Email },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Email = @email, Password = @password WHERE Id = @UserId AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(1234, user);

            Assert.True(result);
        }

        [Fact]
        public void Update_WithoutNewUsernameParamsReturnFalse()
        {
            PatchUser user = new PatchUser(oldPassword: "test2", email: "test3", newPassword: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@UserId", 1234 },
                { "@email", user.Email },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Email = @email, Password = @password WHERE Id = @UserId AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(1234, user);

            Assert.False(result);
        }

        [Fact]
        public void Update_WithoutEmailParamsReturnTrue()
        {
            PatchUser user = new PatchUser(oldPassword: "test2", newUsername: "test3", newPassword: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@UserId", 1234 },
                { "@newUsername", user.NewUsername },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Username = @newUsername, Password = @password WHERE Id = @UserId AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(1234, user);

            Assert.True(result);
        }

        [Fact]
        public void Update_WithoutEmailParamsReturnFalse()
        {
            PatchUser user = new PatchUser(oldPassword: "test2", newUsername: "test3", newPassword: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@UserId", 1234 },
                { "@newUsername", user.NewUsername },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Username = @newUsername, Password = @password WHERE Id = @UserId AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(1234, user);

            Assert.False(result);
        }

        [Fact]
        public void Update_WithoutNewPasswordParamsReturnTrue()
        {
            PatchUser user = new PatchUser(oldPassword: "test2", newUsername: "test3", email: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@UserId", 1234 },
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
            };

            string query = "UPDATE Users SET Username = @newUsername, Email = @email WHERE Id = @UserId AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(1234, user);

            Assert.True(result);
        }

        [Fact]
        public void Update_WithoutNewPasswordParamsReturnFalse()
        {
            PatchUser user = new PatchUser(oldPassword: "test2", newUsername: "test3", email: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@UserId", 1234 },
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
            };

            string query = "UPDATE Users SET Username = @newUsername, Email = @email WHERE Id = @UserId AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(1234, user);

            Assert.False(result);
        }
    }
}
