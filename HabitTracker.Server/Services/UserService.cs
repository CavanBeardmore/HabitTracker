using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Auth;
using HabitTracker.Server.Exceptions;
using System.Text.Json;

namespace HabitTracker.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHabitRepository _habitRepository;
        private readonly IHabitLogRepository _habitLogRepository;
        private readonly IPasswordService _passwordService;
        private readonly IAuthentication _auth;

        public UserService(IUserRepository userRepository, IHabitLogRepository habitLogRepository, IHabitRepository HabitRepository, IPasswordService passwordService, IAuthentication auth)
        {
            _userRepository = userRepository;
            _habitLogRepository = habitLogRepository;
            _habitRepository = HabitRepository; 
            _passwordService = passwordService;
            _auth = auth;
        }

        public User? GetByUsername(string username)
        {
            try
            {
                User? user = _userRepository.GetByUsername(username);

                if (user == null)
                {
                    throw new NotFoundException($"User of - {username} - was not found");
                }

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
                var existingUser = _userRepository.GetByUsername(user.Username);

                if (existingUser != null)
                {
                    throw new ConflictException($"User already exists - {user.Username}");
                }

                user.Password = _passwordService.HashPassword(user.Password);

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
                var existingUser = _userRepository.GetByUsername(user.Username);

                if (existingUser == null)
                {
                    throw new BadRequestException($"User of user id - {userId} - does not exist");
                }

                bool isPasswordValid = _passwordService.VerifyPassword(user.Password, existingUser.Password);

                if (isPasswordValid == false)
                {
                    throw new ForbiddenException("Incorrect password");
                }

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

        public string? Update(int userId, PatchUser user)
        {
            try
            {
                Console.WriteLine(JsonSerializer.Serialize(user));
                var existingUser = _userRepository.GetById(userId);

                if (existingUser == null)
                {
                    throw new BadRequestException($"The user of user id {userId} does not exist");
                }

                bool isPasswordValid = _passwordService.VerifyPassword(user.OldPassword, existingUser.Password);

                if (isPasswordValid == false)
                {
                    throw new ForbiddenException("Incorrect password.");
                }

                if (!string.IsNullOrWhiteSpace(user.NewPassword))
                {
                    user.NewPassword = _passwordService.HashPassword(user.NewPassword);
                }
                Console.WriteLine(JsonSerializer.Serialize(user));
                bool success = _userRepository.Update(userId, user);

                if (success == false)
                {
                    throw new AppException($"Could not update user of user id - {userId}");
                }

                return string.IsNullOrEmpty(user.NewUsername) == false ? _auth.GenerateJWTToken(user.NewUsername) : null;
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
                User? foundUser = _userRepository.GetByUsername(username);
                if (foundUser == null)
                {
                    throw new NotFoundException($"Could not find user from usernae - {username}");
                }

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
