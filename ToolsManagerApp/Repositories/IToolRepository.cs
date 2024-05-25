using System.Collections.Generic;
using System.Threading.Tasks;
using ToolsManagerApp.Models;

namespace ToolsManagerApp.Repositories
{
    public interface IToolRepository
    {
        Task<IEnumerable<Tool>> GetAllToolsAsync();
        Task<Tool> GetToolByIdAsync(string id);
        Task AddToolAsync(Tool tool);
        Task UpdateToolAsync(Tool tool);
        Task DeleteToolAsync(string id);
        Task<IEnumerable<Tool>> GetToolsByIdsAsync(IEnumerable<string> ids); 
    }
}
