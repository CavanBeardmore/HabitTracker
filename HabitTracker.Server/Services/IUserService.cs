using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Services
{
    public interface IUserService
    {
        User? GetByUsername(string username);
        bool Add(PostUser user);
        bool Delete(int userId, AuthUser user);
        Tuple<string?, User>? Update(int userId, PatchUser user);
        bool AreUserCredentialsCorrect(string username, string password);
    }
}
