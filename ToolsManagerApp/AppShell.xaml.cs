using ToolsManagerApp.Views;
using ToolsManagerApp.Services;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ToolsManagerApp;

public partial class AppShell : Shell
{
    public ICommand LogoutCommand { get; }

    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
        Routing.RegisterRoute(nameof(ToolsPage), typeof(ToolsPage));
        Routing.RegisterRoute(nameof(UserToolsPage), typeof(UserToolsPage));

        LogoutCommand = new AsyncRelayCommand(OnLogoutAsync);
        BindingContext = this;

        Navigating += OnNavigating;
    }

    private async Task OnLogoutAsync()
    {
        UserSession.Instance.ClearSession();
        await GoToAsync("//LoginPage");
    }

    private void OnNavigating(object sender, ShellNavigatingEventArgs e)
    {
        if (e.Target.Location.OriginalString == "//LoginPage")
        {
            FlyoutBehavior = FlyoutBehavior.Disabled;
        }
        else
        {
            FlyoutBehavior = FlyoutBehavior.Flyout;
        }
    }
}
