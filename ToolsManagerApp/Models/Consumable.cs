using MongoDB.Bson.Serialization.Attributes;

namespace ToolsManagerApp.Models
{
    public class Consumable : Tool
    {
        [BsonElement("initialValue")]
        public int InitialValue { get; set; }

        [BsonElement("maxValue")]
        public int MaxValue { get; set; }

        [BsonElement("currentValue")]
        public int CurrentValue { get; set; }
    }
}
