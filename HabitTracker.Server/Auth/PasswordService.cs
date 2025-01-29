using BCrypt.Net;

namespace HabitTracker.Server.Auth
{
    public class PasswordService : IPasswordService
    {
        public PasswordService() { }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        }
    }
}
