using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using System.Data.SQLite;

namespace HabitTracker.Server.Repository
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        bool Add(PostUser user);
        bool Update(PatchUser user);
        bool Delete(string username);
    }
}
