using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Services
{
    public interface IUserService
    {
        User? Get(string username);
        User? Get(int id);
        bool Add(PostUser user);
        bool Delete(int userId, AuthUser user);
        UpdatedUserResult? Update(int userId, PatchUser user);
        bool AreUserCredentialsCorrect(string username, string password);
    }
}
