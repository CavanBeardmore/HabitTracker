using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Auth;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IAuthentication _auth;
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository, IPasswordService passwordService, IAuthentication auth)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _auth = auth;
            _logger = logger;
        }

        public User? Get(int id)
        {
            try
            {
                _logger.LogInformation("UserService - Get - getting user by id");
                User? user = _userRepository.GetById(id);

                if (user == null)
                {
                    throw new NotFoundException($"User of ID: - {id} - was not found");
                }

                _logger.LogInformation("UserService - Get - found user");
                return user;
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when fetching the user from Id:  - {id}");
            }
        }

        public User? Get(string username)
        {
            try
            {
                _logger.LogInformation("UserService - get - getting user by username");
                User? user = _userRepository.GetByUsername(username);

                if (user == null)
                {
                    throw new NotFoundException($"User of - {username} - was not found");
                }

                _logger.LogInformation("UserService - Get - found user");
                return user;
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when fetching the user from username  - {username}");
            }
        }

        public bool Add(PostUser user)
        {
            try
            {
                _logger.LogInformation("UserService - Add - checking if user exists already");
                var existingUser = _userRepository.GetByUsername(user.Username);

                if (existingUser != null)
                {
                    throw new ConflictException($"User already exists - {user.Username}");
                }

                user.Password = _passwordService.HashPassword(user.Password);

                _logger.LogInformation("UserService - Add - adding user");
                return _userRepository.Add(user);
            }
            catch (ConflictException ex)
            {
                throw new ConflictException(ex.Message);
            }
            catch
            {
                throw new AppException("An error occured when creating the user.");
            }
        }

        public bool Delete(int userId, AuthUser user)
        {
            try
            {
                _logger.LogInformation("UserService - Delete - checking if user exists");
                var existingUser = _userRepository.GetByUsername(user.Username);

                if (existingUser == null)
                {
                    throw new BadRequestException($"User of user id - {userId} - does not exist");
                }

                _logger.LogInformation("UserService - Delete - verifying user password");
                bool isPasswordValid = _passwordService.VerifyPassword(user.Password, existingUser.Password);

                if (isPasswordValid == false)
                {
                    throw new ForbiddenException("Incorrect password");
                }

                _logger.LogInformation("UserService - Delete - user password is valid deleting user");
                return _userRepository.Delete(userId);
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                throw new ForbiddenException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when deleting the user of user id - {userId}");
            }
        }

        public UpdatedUserResult? Update(int userId, PatchUser user)
        {
            try
            {
                _logger.LogInformation("UserService - Update - checking if user exists");
                var existingUser = _userRepository.GetById(userId);

                if (existingUser == null)
                {
                    throw new BadRequestException($"The user of user id {userId} does not exist");
                }

                _logger.LogInformation("UserService - Update - verifying user password");
                bool isPasswordValid = _passwordService.VerifyPassword(user.OldPassword, existingUser.Password);

                if (isPasswordValid == false)
                {
                    throw new ForbiddenException("Incorrect password.");
                }

                _logger.LogInformation("UserService - Update - password is valid");

                if (!string.IsNullOrWhiteSpace(user.NewPassword))
                {
                    _logger.LogInformation("UserService - Update - hashing new password valid");
                    user.NewPassword = _passwordService.HashPassword(user.NewPassword);
                }

                _logger.LogInformation("UserService - Update - updating user");
                User? updatedUser = _userRepository.Update(userId, user);

                if (updatedUser == null)
                {
                    throw new AppException($"Could not update user of user id - {userId}");
                }

                _logger.LogInformation("UserService - Update - updated user {@User}", updatedUser);
                return string.IsNullOrEmpty(user.NewUsername) == false ? new UpdatedUserResult(_auth.GenerateJWTToken(user.NewUsername), updatedUser) : null;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                throw new ForbiddenException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new AppException($"An error occured when updating the user of user id {userId} - {ex.Message}");
            }
        }

        public bool AreUserCredentialsCorrect(string username, string password)
        {
            try
            {
                _logger.LogInformation("UserService - AreUserCredentialsCorrect - getting user");
                User? foundUser = _userRepository.GetByUsername(username);

                if (foundUser == null)
                {
                    throw new NotFoundException($"Could not find user from username - {username}");
                }

                _logger.LogInformation("UserService - AreUserCredentialsCorrect - verifying password");
                return _passwordService.VerifyPassword(password, foundUser.Password);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when checking if credentials are correct for user of username {username}");
            }
        }
    }
}
