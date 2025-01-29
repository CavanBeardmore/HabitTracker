using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Auth;
using HabitTracker.Server.Database.Entities;

namespace HabitTracker.Server.Services
{
    public class GetUserByUsernameResponse : IServiceResponseWithDataAndStatusCode<User?>
    {
        public bool Success { get; }
        public User? Data { get; }
        public string? Error { get; }
        public EStatusCodes? StatusCode { get; }

        public GetUserByUsernameResponse(bool success, User? data, string? error, EStatusCodes statusCode) 
        {
            Success = success;
            Data = data;
            Error = error;
            StatusCode = statusCode;
        }
    }

    public class UserResponse : IServiceResponseWithStatusCode
    {
        public bool Success { get; }
        public string? Error { get; }
        public EStatusCodes? StatusCode { get; }

        public UserResponse(bool success, string? error, EStatusCodes? statusCode = EStatusCodes.INTERNAL_SERVER_ERROR)
        {
            Success = success;
            Error = error;
            StatusCode = statusCode;
        }
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public UserService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public IServiceResponseWithDataAndStatusCode<User?> GetByUsername(string username)
        {
            try
            {
                User? user = _userRepository.GetByUsername(username);

                if (user == null)
                {
                    return new GetUserByUsernameResponse(false, null, "User not found", EStatusCodes.NOT_FOUND);
                }

                return new GetUserByUsernameResponse(true, user, null, EStatusCodes.OK);
            }
            catch (Exception ex)
            {
                return new GetUserByUsernameResponse(false, null, ex.Message, EStatusCodes.INTERNAL_SERVER_ERROR);
            }
        }

        public IServiceResponseWithStatusCode Add(PostUser user)
        {
            try
            {
                var existingUser = _userRepository.GetByUsername(user.Username);

                if (existingUser != null)
                {
                    return new UserResponse(false, "User already exists", EStatusCodes.CONFLICT);
                }

                user.Password = _passwordService.HashPassword(user.Password);

                bool success = _userRepository.Add(user);

                return new UserResponse(success, null, success ? EStatusCodes.CREATED : EStatusCodes.INTERNAL_SERVER_ERROR);
            }
            catch (Exception ex)
            {
                return new UserResponse(false, ex.Message);
            }
        }

        public IServiceResponseWithStatusCode Delete(AuthUser user)
        {
            try
            {
                var existingUser = _userRepository.GetByUsername(user.Username);

                if (existingUser == null)
                {
                    return new UserResponse(false, "User doesn't exist", EStatusCodes.BAD_REQUEST);
                }

                bool matches = _passwordService.VerifyPassword(user.Password, existingUser.Password);

                if (matches == false)
                {
                    return new UserResponse(false, "Incorrect Password", EStatusCodes.FORBIDDEN);
                }

                return new UserResponse(_userRepository.Delete(user.Username), null);
            }
            catch (Exception ex)
            {
                return new UserResponse(false, ex.Message);
            }
        }

        public IServiceResponseWithStatusCode Update(PatchUser user)
        {
            try
            {
                var existingUser = _userRepository.GetByUsername(user.OldUsername);

                if (existingUser == null)
                {
                    return new UserResponse(false, "User doesn't exist", EStatusCodes.BAD_REQUEST);
                }

                bool matches = _passwordService.VerifyPassword(user.OldPassword, existingUser.Password);

                if (matches == false)
                {
                    return new UserResponse(false, "Incorrect Password", EStatusCodes.FORBIDDEN);
                }

                if (user.NewPassword != null)
                {
                    user.NewPassword = _passwordService.HashPassword(user.NewPassword);
                }

                return new UserResponse(_userRepository.Update(user), null);
            }
            catch (Exception ex)
            {
                return new UserResponse(false, ex.Message);
            }
        }

        public IServiceResponseWithStatusCode AreUserCredentialsCorrect(string username, string password)
        {
            try
            {
                User? foundUser = _userRepository.GetByUsername(username);
                if (foundUser == null)
                {
                    return new UserResponse(false, "User not found", EStatusCodes.NOT_FOUND);
                }

                bool matches = _passwordService.VerifyPassword(password, foundUser.Password);
                return new UserResponse(matches, null, matches ? EStatusCodes.OK : EStatusCodes.UNAUTHORIZED);
            }
            catch
            {
                return new UserResponse(false, null, EStatusCodes.INTERNAL_SERVER_ERROR);
            }
        }
    }
}
