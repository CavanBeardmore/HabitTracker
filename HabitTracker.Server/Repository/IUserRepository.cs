using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using System.Data.SQLite;

namespace HabitTracker.Server.Repository
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        User? GetById(int userId);
        bool Add(PostUser user);
        bool Update(int userId, PatchUser user);
        bool Delete(int userId);
    }
}
