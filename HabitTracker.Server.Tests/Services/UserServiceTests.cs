using Microsoft.Extensions.Logging;
using HabitTracker.Server.Auth;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Services;
using Moq;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Models;
using System.Data.Entity.Core.Metadata.Edm;

namespace HabitTracker.Server.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _service;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly Mock<IAuthentication> _mockAuthentication;
        private readonly Mock<ILogger<UserService>> _mockLogger;

        public UserServiceTests()
        {
            _mockAuthentication = new Mock<IAuthentication>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordService = new Mock<IPasswordService>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _service = new UserService(_mockLogger.Object, _mockUserRepository.Object, _mockPasswordService.Object, _mockAuthentication.Object);
        }

        [Fact]
        public void GetByUsername_ReturnsUser()
        {
            string username = "test";

            _mockUserRepository.Setup(repository => repository.GetByUsername(username)).Returns(new User(1234, username, "test2", "test3"));

            var result = _service.GetByUsername(username);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.Username == username);
            Assert.True(result.Email == "test2");
            Assert.True(result.Password == "test3");
        }

        [Fact]
        public void GetByUsername_ThrowsNotFoundException()
        {
            string username = "test";

            _mockUserRepository.Setup(repository => repository.GetByUsername(username)).Returns((User)null);

            Assert.Throws<NotFoundException>(() => _service.GetByUsername(username));
        }
        [Fact]
        public void GetByUsername_ThrowsAppException()
        {
            string username = "test";

            _mockUserRepository.Setup(repository => repository.GetByUsername(username)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.GetByUsername(username));
        }

        [Fact]
        public void Add_ReturnsTrue()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns((User)null);

            _mockPasswordService.Setup(service => service.HashPassword("test3")).Returns("test3");

            _mockUserRepository.Setup(repository => repository.Add(user)).Returns(true);

            var result = _service.Add(user);

            Assert.True(result);
        }

        [Fact]
        public void Add_ReturnsFalse()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns((User)null);

            _mockPasswordService.Setup(service => service.HashPassword("test3")).Returns("test3");

            _mockUserRepository.Setup(repository => repository.Add(user)).Returns(false);

            var result = _service.Add(user);

            Assert.False(result);
        }

        [Fact]
        public void Add_ThrowsConflictException()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns(new User(1234, "test1", "test2", "test3"));

            _mockPasswordService.Setup(service => service.HashPassword("test3")).Returns("test3");

            _mockUserRepository.Setup(repository => repository.Add(user)).Returns(false);

            Assert.Throws<ConflictException>(() => _service.Add(user));
        }

        [Fact]
        public void Add_ThrowsAppException()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns((User)null);

            _mockPasswordService.Setup(service => service.HashPassword("test3")).Returns("test3");

            _mockUserRepository.Setup(repository => repository.Add(user)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.Add(user));
        }

        [Fact]
        public void Delete_ReturnsTrue()
        {
            AuthUser user = new AuthUser("test1", "test2");
            int userId = 1234;

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test2", "test3")).Returns(true);

            _mockUserRepository.Setup(repository => repository.Delete(userId)).Returns(true);

            var result = _service.Delete(userId, user);

            Assert.True(result);
        }

        [Fact]
        public void Delete_ReturnsFalse()
        {
            AuthUser user = new AuthUser("test1", "test2");
            int userId = 1234;

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test2", "test3")).Returns(true);

            _mockUserRepository.Setup(repository => repository.Delete(userId)).Returns(false);

            var result = _service.Delete(userId, user);

            Assert.False(result);
        }

        [Fact]
        public void Delete_ThrowsBadRequestException()
        {
            AuthUser user = new AuthUser("test1", "test2");
            int userId = 1234;

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns((User)null);
            _mockPasswordService.Setup(service => service.VerifyPassword("test2", "test3")).Returns(true);

            _mockUserRepository.Setup(repository => repository.Delete(userId)).Returns(false);

            Assert.Throws<BadRequestException>(() => _service.Delete(userId, user));
        }

        [Fact]
        public void Delete_ThrowsForbiddenException()
        {
            AuthUser user = new AuthUser("test1", "test2");
            int userId = 1234;

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test2", "test3")).Returns(false);

            _mockUserRepository.Setup(repository => repository.Delete(userId)).Returns(false);

            Assert.Throws<ForbiddenException>(() => _service.Delete(userId, user));
        }

        [Fact]
        public void Delete_ThrowsAppException()
        {
            AuthUser user = new AuthUser("test1", "test2");
            int userId = 1234;

            _mockUserRepository.Setup(repository => repository.GetByUsername("test1")).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test2", "test3")).Returns(true);

            _mockUserRepository.Setup(repository => repository.Delete(userId)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.Delete(userId, user));
        }

        [Fact]
        public void Update_ReturnsJwtAfterHashingPassword()
        {
            int userId = 1234;
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            _mockUserRepository.Setup(repository => repository.GetById(userId)).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test1", "test3")).Returns(true);
            _mockPasswordService.Setup(service => service.HashPassword("test4")).Returns("hashed-test");

            user.NewPassword = "hashed-test";

            _mockUserRepository.Setup(repository => repository.Update(userId, user)).Returns(true);

            _mockAuthentication.Setup(auth => auth.GenerateJWTToken("test2")).Returns("test1234");

            var result = _service.Update(userId, user);

            Assert.True(result == "test1234");
        }

        [Fact]
        public void Update_ReturnsJwt()
        {
            int userId = 1234;
            PatchUser user = new PatchUser("test1", "test2", "test3");

            _mockUserRepository.Setup(repository => repository.GetById(userId)).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test1", "test3")).Returns(true);

            _mockUserRepository.Setup(repository => repository.Update(userId, user)).Returns(true);

            _mockAuthentication.Setup(auth => auth.GenerateJWTToken("test2")).Returns("test1234");

            var result = _service.Update(userId, user);

            Assert.True(result == "test1234");
        }

        [Fact]
        public void Update_ReturnsNull()
        {
            int userId = 1234;
            PatchUser user = new PatchUser("test1", email: "test3", newPassword: "test4");

            _mockUserRepository.Setup(repository => repository.GetById(userId)).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test1", "test3")).Returns(true);
            _mockPasswordService.Setup(service => service.HashPassword("test4")).Returns("test4");
            _mockUserRepository.Setup(repository => repository.Update(userId, user)).Returns(true);

            var result = _service.Update(userId, user);

            Assert.True(result == null);
        }

        [Fact]
        public void Update_ThrowsBadRequestException()
        {
            int userId = 1234;
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            _mockUserRepository.Setup(repository => repository.GetById(userId)).Returns((User)null);

            Assert.Throws<BadRequestException>(() => _service.Update(userId, user));
        }

        [Fact]
        public void Update_ThrowsForbiddenException()
        {
            int userId = 1234;
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            _mockUserRepository.Setup(repository => repository.GetById(userId)).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test1", "test3")).Returns(false);

            Assert.Throws<ForbiddenException>(() => _service.Update(userId, user));
        }

        [Fact]
        public void Update_ThrowsAppExceptionIfUpdateFails()
        {
            int userId = 1234;
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            _mockUserRepository.Setup(repository => repository.GetById(userId)).Returns(new User(1234, "test1", "test2", "test3"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test1", "test3")).Returns(true);
            _mockPasswordService.Setup(service => service.HashPassword("test4")).Returns("hashed-test");

            user.NewPassword = "hashed-test";

            _mockUserRepository.Setup(repository => repository.Update(userId, user)).Returns(false);

            Assert.Throws<AppException>(() => _service.Update(userId, user));
        }


        [Fact]
        public void Update_ThrowsAppException()
        {
            int userId = 1234;
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            _mockUserRepository.Setup(repository => repository.GetById(userId)).Throws<Exception>(() => throw new Exception("test"));

            Assert.Throws<AppException>(() => _service.Update(userId, user));
        }

        [Fact]
        public void AreUserCredentialsCorrect_ReturnsTrue()
        {
            string username = "test1";
            string password = "test2";

            _mockUserRepository.Setup(repository => repository.GetByUsername(username)).Returns(new User(1234, "test1", "test", "test2"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test2", "test2")).Returns(true);

            var result = _service.AreUserCredentialsCorrect(username, password);

            Assert.True(result);
        }

        [Fact]
        public void AreUserCredentialsCorrect_ReturnsFalse()
        {
            string username = "test1";
            string password = "test2";

            _mockUserRepository.Setup(repository => repository.GetByUsername(username)).Returns(new User(1234, "test1", "test", "test2"));
            _mockPasswordService.Setup(service => service.VerifyPassword("test2", "test2")).Returns(false);

            var result = _service.AreUserCredentialsCorrect(username, password);

            Assert.False(result);
        }

        [Fact]
        public void AreUserCredentialsCorrect_ThrowsNotFoundException()
        {
            string username = "test1";
            string password = "test2";

            _mockUserRepository.Setup(repository => repository.GetByUsername(username)).Returns((User)null);

            Assert.Throws<NotFoundException>(() => _service.AreUserCredentialsCorrect(username, password));
        }

        [Fact]
        public void AreUserCredentialsCorrect_ThrowsAppException()
        {
            string username = "test1";
            string password = "test2";

            _mockUserRepository.Setup(repository => repository.GetByUsername(username)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.AreUserCredentialsCorrect(username, password));
        }
    }
}
