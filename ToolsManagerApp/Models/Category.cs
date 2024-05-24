namespace ToolsManagerApp.Models
{
    public class Category
    {
        public int Id { get; set; }
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
