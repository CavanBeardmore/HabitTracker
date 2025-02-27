namespace HabitTracker.Server.Auth
{
    public interface IAuthentication
    {
        string GenerateJWTToken(string username);
    }
}
