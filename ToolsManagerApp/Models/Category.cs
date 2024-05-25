using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToolsManagerApp.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }

        public void AddCategory() { /* Implementation */ }
        public void EditCategory(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
