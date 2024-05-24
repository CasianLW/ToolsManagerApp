namespace ToolsManagerApp.Models
{
    public class Tool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public StatusEnum Status { get; set; }
        public int UserAssignedId { get; set; }
        public string QRCode { get; set; }

        public void CheckStatus() { /* Implementation */ }
        public void AssignToUser(int userId) { UserAssignedId = userId; }
        public void RemoveFromUser() { UserAssignedId = 0; }
    }
}
