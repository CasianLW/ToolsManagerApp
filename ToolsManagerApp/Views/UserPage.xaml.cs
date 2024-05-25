using ToolsManagerApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ToolsManagerApp.Views
{
    public partial class UserPage : ContentPage
    {
        public UserPage()
        {
            InitializeComponent();
            BindingContext = App.ServiceProvider.GetService<UsersViewModel>();
        }
    }
}
