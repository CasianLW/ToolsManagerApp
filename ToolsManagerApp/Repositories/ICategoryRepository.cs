using System.Collections.Generic;
using System.Threading.Tasks;
using ToolsManagerApp.Models;

namespace ToolsManagerApp.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(string id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(string id);

    }
}
