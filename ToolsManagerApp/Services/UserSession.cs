using ToolsManagerApp.Models;

namespace ToolsManagerApp.Services
{
    public class UserSession
    {
        private static UserSession _instance;
        private static readonly object _lock = new object();

        private UserSession() { }

        public static UserSession Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserSession();
                    }
                    return _instance;
                }
            }
        }

        public string Email { get; set; }
        public RoleEnum Role { get; set; }

        public void ClearSession()
        {
            Email = string.Empty;
            Role = RoleEnum.Employee; // Default role or you can set it to null if Role is nullable
        }
    }
}
