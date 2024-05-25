using ToolsManagerApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace ToolsManagerApp.Views
{
    public partial class CategoriesPage : ContentPage
    {
        public CategoriesPage()
        {
            InitializeComponent();
            BindingContext = App.ServiceProvider.GetService<CategoriesViewModel>();
        }
    }
}
