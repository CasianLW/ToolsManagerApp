using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToolsManagerApp.Models;
using Microsoft.Extensions.Logging;

namespace ToolsManagerApp.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMongoCollection<Category> _categories;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(IMongoDatabase database, ILogger<CategoryRepository> logger)
        {
            _categories = database.GetCollection<Category>("Categories");
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categories.Find(category => true).ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(string id)
        {
            return await _categories.Find(category => category.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _categories.InsertOneAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _categories.ReplaceOneAsync(c => c.Id == category.Id, category);
        }

        public async Task DeleteCategoryAsync(string id)
        {
            await _categories.DeleteOneAsync(category => category.Id == id);
        }
    }
}
