namespace ToolsManagerApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoleEnum Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public void Authenticate() { /* Implementation */ }
        public void ChangePassword(string newPassword) { Password = newPassword; }
        public void UpdateInfos(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
