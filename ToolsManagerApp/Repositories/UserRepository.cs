using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToolsManagerApp.Models;
using Microsoft.Extensions.Logging;

namespace ToolsManagerApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IMongoDatabase database, ILogger<UserRepository> logger)
        {
            _users = database.GetCollection<User>("Users");
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            return await _users.Find<User>(user => user.Email == email && user.Password == password).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find<User>(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(user => user.Id == id);
        }

        public async Task EnsureAdminUserAsync()
        {
            var admin = await _users.Find(u => u.Email == "admin@example.com").FirstOrDefaultAsync();
            if (admin == null)
            {
                await AddUserAsync(new User
                {
                    Name = "Admin",
                    Email = "admin@example.com",
                    Password = "adminpassword",
                    Role = RoleEnum.Admin
                });
            }
        }
    }
}
