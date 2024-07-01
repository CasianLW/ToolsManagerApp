using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToolsManagerApp.Models;
using Microsoft.Extensions.Logging;

namespace ToolsManagerApp.Repositories
{
    public class ToolRepository : IToolRepository
    {
        private readonly IMongoCollection<Tool> _tools;
        private readonly IMongoCollection<Consumable> _consumables;
        private readonly ILogger<ToolRepository> _logger;

        public ToolRepository(IMongoDatabase database, ILogger<ToolRepository> logger)
        {
            _tools = database.GetCollection<Tool>("Tools");
            _consumables = database.GetCollection<Consumable>("Consumables");
            _logger = logger;
        }

        // Tool methods
        public async Task<IEnumerable<Tool>> GetAllToolsAsync()
        {
            return await _tools.Find(_ => true).ToListAsync();
        }

        public async Task<Tool> GetToolByIdAsync(string id)
        {
            return await _tools.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddToolAsync(Tool tool)
        {
            await _tools.InsertOneAsync(tool);
        }

        public async Task UpdateToolAsync(Tool tool)
        {
            await _tools.ReplaceOneAsync(t => t.Id == tool.Id, tool);
        }

        public async Task DeleteToolAsync(string id)
        {
            await _tools.DeleteOneAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tool>> GetToolsByIdsAsync(IEnumerable<string> toolIds)
        {
            return await _tools.Find(t => toolIds.Contains(t.Id)).ToListAsync();
        }

        // Consumable methods
        public async Task<IEnumerable<Consumable>> GetAllConsumablesAsync()
        {
            return await _consumables.Find(_ => true).ToListAsync();
        }

        public async Task<Consumable> GetConsumableByIdAsync(string id)
        {
            return await _consumables.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddConsumableAsync(Consumable consumable)
        {
            await _consumables.InsertOneAsync(consumable);
        }

        public async Task UpdateConsumableAsync(Consumable consumable)
        {
            await _consumables.ReplaceOneAsync(c => c.Id == consumable.Id, consumable);
        }

        public async Task DeleteConsumableAsync(string id)
        {
            await _consumables.DeleteOneAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Consumable>> GetConsumablesByIdsAsync(IEnumerable<string> consumableIds)
        {
            return await _consumables.Find(c => consumableIds.Contains(c.Id)).ToListAsync();
        }
    }
}
