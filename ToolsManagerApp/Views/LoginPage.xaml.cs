// using ToolsManagerApp.Models;
// using ToolsManagerApp.Repositories;
// using ToolsManagerApp.ViewModels;

// namespace ToolsManagerApp.Views
// {
//     public partial class LoginPage : ContentPage
//     {
//         // public LoginPage(LoginViewModel viewModel)
//         public LoginPage()
//         {
//             InitializeComponent();
//             // BindingContext = viewModel;
// 			// BindingContext = new LoginViewModel(new UserRepository(new AppDbContext()));
// 			            // BindingContext = App.Current.Services.GetService<LoginViewModel>() ?? new LoginViewModel(new UserRepository(new AppDbContext()));

//             BindingContext = ServiceProvider.Instance.GetService<LoginViewModel>();




//         }
//     }
// }
using ToolsManagerApp.Repositories;
using ToolsManagerApp.ViewModels;

namespace ToolsManagerApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = App.ServiceProvider.GetService<LoginViewModel>() ?? new LoginViewModel(App.ServiceProvider.GetService<IUserRepository>());

        }
    }
}