using ToolsManagerApp.Models;

namespace ToolsManagerApp.Services
{
    public class UserFactory
    {
        public UserFactory() { }

        public User InstantiateUser(RoleEnum role, string name, string email, string password)
        {
            User user = new User
            {
                Role = role,
                Name = name,
                Email = email,
                Password = password // Consider hashing this
            };
            return user;
        }
    }
}
