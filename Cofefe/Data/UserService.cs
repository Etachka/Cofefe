using Cofefe.Models;

namespace Cofefe.Data
{
    public class UserService
    {
        private List<User> _users;

        private readonly ApplicationContext _context;

        public UserService(ApplicationContext context)
        {
            _context = context;
            _users = _context.Users.ToList();
        }
        public User GetUser(string login, string password)
        {
            return _users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }
    }
}
