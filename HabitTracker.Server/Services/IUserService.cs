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
        IServiceResponseWithStatusCode Delete(AuthUser user);
        IServiceResponseWithStatusCode Update(PatchUser user);
    }
}
