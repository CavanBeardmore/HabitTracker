using HabitTracker.Server.Database;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Facade;
using HabitTracker.Server.Models;
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

            string query = "SELECT * FROM Users WHERE Users.Username = @Username AND IsDeleted = 0;";

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

            string query = "SELECT * FROM Users WHERE Users.Username = @Username AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(new List<User>());

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
            string username = "test";

            string query = "UPDATE Users SET IsDeleted = 1 WHERE Username = @Username AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Delete(username);

            Assert.True(result);
        }

        [Fact]
        public void Delete_ReturnsFalse()
        {
            string username = "test";

            string query = "UPDATE Users SET IsDeleted = 1 WHERE Username = @Username AND IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Delete(username);

            Assert.False(result);
        }

        [Fact]
        public void Update_WithAllParamsReturnTrue()
        {
            PatchUser user = new PatchUser("test", "test1", "test2", "test3", "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername },
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Username = @newUsername, Email = @email, Password = @password WHERE Username = @oldUsername AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(user);

            Assert.True(result);
        }

        [Fact]
        public void Update_WithAllParamsReturnFalse()
        {
            PatchUser user = new PatchUser("test", "test1", "test2", "test3", "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername },
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Username = @newUsername, Email = @email, Password = @password WHERE Username = @oldUsername AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(user);

            Assert.False(result);
        }

        [Fact]
        public void Update_WithoutNewUsernameParamsReturnTrue()
        {
            PatchUser user = new PatchUser(oldUsername: "test", oldPassword: "test2", email: "test3", newPassword: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername },
                { "@email", user.Email },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Email = @email, Password = @password WHERE Username = @oldUsername AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(user);

            Assert.True(result);
        }

        [Fact]
        public void Update_WithoutNewUsernameParamsReturnFalse()
        {
            PatchUser user = new PatchUser(oldUsername: "test", oldPassword: "test2", email: "test3", newPassword: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername },
                { "@email", user.Email },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Email = @email, Password = @password WHERE Username = @oldUsername AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(user);

            Assert.False(result);
        }

        [Fact]
        public void Update_WithoutEmailParamsReturnTrue()
        {
            PatchUser user = new PatchUser(oldUsername: "test", oldPassword: "test2", newUsername: "test3", newPassword: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername },
                { "@newUsername", user.NewUsername },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Username = @newUsername, Password = @password WHERE Username = @oldUsername AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(user);

            Assert.True(result);
        }

        [Fact]
        public void Update_WithoutEmailParamsReturnFalse()
        {
            PatchUser user = new PatchUser(oldUsername: "test", oldPassword: "test2", newUsername: "test3", newPassword: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername },
                { "@newUsername", user.NewUsername },
                { "@password", user.NewPassword }
            };

            string query = "UPDATE Users SET Username = @newUsername, Password = @password WHERE Username = @oldUsername AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(user);

            Assert.False(result);
        }

        [Fact]
        public void Update_WithoutNewPasswordParamsReturnTrue()
        {
            PatchUser user = new PatchUser(oldUsername: "test", oldPassword: "test2", newUsername: "test3", email: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername },
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
            };

            string query = "UPDATE Users SET Username = @newUsername, Email = @email WHERE Username = @oldUsername AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(user);

            Assert.True(result);
        }

        [Fact]
        public void Update_WithoutNewPasswordParamsReturnFalse()
        {
            PatchUser user = new PatchUser(oldUsername: "test", oldPassword: "test2", newUsername: "test3", email: "test4");

            Dictionary<string, object> parameters = new Dictionary<string, object>{
                { "@oldUsername", user.OldUsername },
                { "@newUsername", user.NewUsername },
                { "@email", user.Email },
            };

            string query = "UPDATE Users SET Username = @newUsername, Email = @email WHERE Username = @oldUsername AND IsDeleted = 0;";

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(user);

            Assert.False(result);
        }
    }
}
