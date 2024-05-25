using System.Collections.Generic;
using System.Threading.Tasks;
using ToolsManagerApp.Models;

namespace ToolsManagerApp.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByEmailAndPasswordAsync(string email, string password);
        Task<User> GetUserByEmailAsync(string email);

        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string id);
        Task EnsureAdminUserAsync();
    }
}
