using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToolsManagerApp.Models
{
    public class Tool
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public StatusEnum Status { get; set; }
        public string UserAssignedId { get; set; }
        public string QRCode { get; set; }

        public void CheckStatus() { /* Implementation */ }
        public void AssignToUser(string userId) { UserAssignedId = userId; }
        public void RemoveFromUser() { UserAssignedId = null; }
    }
}
