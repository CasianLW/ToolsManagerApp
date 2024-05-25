using ToolsManagerApp.ViewModels;

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
