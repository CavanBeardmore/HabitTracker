using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Services.Responses;
using Microsoft.AspNetCore.Components.Web;

namespace HabitTracker.Server.Services
{
    public interface IUserService
    {
        IServiceResponseWithDataAndStatusCode<User?> GetByUsername(string username);
        IServiceResponseWithStatusCode Add(PostUser user);
        IServiceResponseWithStatusCode Delete(int userId, AuthUser user);
        IServiceResponseWithStatusCode Update(int userId, PatchUser user);
    }
}
