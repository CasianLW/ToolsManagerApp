using ToolsManagerApp.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ToolsManagerApp.Views
{
    public partial class ToolsPage : ContentPage
    {
        public ToolsPage()
        {
            InitializeComponent();
            BindingContext = App.ServiceProvider.GetService<ToolsViewModel>();
        }
    }
}
