using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolsManagerApp.Models;
using ToolsManagerApp.Services;
using System.Collections.ObjectModel;

namespace ToolsManagerApp.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        private readonly UserFactory _userFactory;
        private ObservableCollection<User> _users;

        public UserViewModel()
        {
            _userFactory = new UserFactory();
            _users = new ObservableCollection<User>();
        }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [RelayCommand]
        public void CreateUser()
        {
            var newUser = _userFactory.InstantiateUser(RoleEnum.Employee, Name, Email, Password);
            _users.Add(newUser);
        }

        public ObservableCollection<User> Users => _users;
    }
}
