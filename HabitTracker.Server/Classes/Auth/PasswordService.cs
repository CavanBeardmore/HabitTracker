using BCrypt.Net;

namespace HabitTracker.Server.Classes.PasswordService
{
    public class PasswordService
    {
        private readonly string _password;

        public PasswordService(string password)
        {  
            _password = password; 
        }

        public string HashPassword()
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(_password);
        }

        public bool VerifyPassword(string hashedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(hashedPassword, _password);
        }
    }
}
