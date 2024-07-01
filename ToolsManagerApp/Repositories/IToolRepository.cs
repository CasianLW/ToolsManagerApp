using System.Collections.Generic;
using System.Threading.Tasks;
using ToolsManagerApp.Models;

namespace ToolsManagerApp.Repositories
{
    public interface IToolRepository
    {
        // Tool methods
        Task<IEnumerable<Tool>> GetAllToolsAsync();
        Task<Tool> GetToolByIdAsync(string id);
        Task AddToolAsync(Tool tool);
        Task UpdateToolAsync(Tool tool);
        Task DeleteToolAsync(string id);
        Task<IEnumerable<Tool>> GetToolsByIdsAsync(IEnumerable<string> toolIds);

        // Consumable methods
        Task<IEnumerable<Consumable>> GetAllConsumablesAsync();
        Task<Consumable> GetConsumableByIdAsync(string id);
        Task AddConsumableAsync(Consumable consumable);
        Task UpdateConsumableAsync(Consumable consumable);
        Task DeleteConsumableAsync(string id);
        Task<IEnumerable<Consumable>> GetConsumablesByIdsAsync(IEnumerable<string> consumableIds);
    }
}
