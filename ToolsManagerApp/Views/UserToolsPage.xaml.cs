using ToolsManagerApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ToolsManagerApp.Views
{
    public partial class UserToolsPage : ContentPage
    {
        public UserToolsPage()
        {
            InitializeComponent();
            BindingContext = App.ServiceProvider.GetRequiredService<UserToolsViewModel>();
        }
    }
}
