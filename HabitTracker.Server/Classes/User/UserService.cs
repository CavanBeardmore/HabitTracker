namespace HabitTracker.Server.Classes.User
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User? GetByUsername(string username)
        {
            return _userRepository.GetByUsername(username);
        }

        public void AddUser(User user)
        {
            _userRepository.Add(user);
        }

        public void UpdateUser(User user, string oldUsername)
        {
            _userRepository.Update(user, oldUsername);
        }

        public void DeleteUser(string username)
        {
            _userRepository.Delete(username);
        }
    }
}
