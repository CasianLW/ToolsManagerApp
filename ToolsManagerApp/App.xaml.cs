// using ToolsManagerApp.Views;
// using Microsoft.Extensions.DependencyInjection;

// namespace ToolsManagerApp;

// public partial class App : Application
// {
// 	public App()
// 	{
// 		InitializeComponent();

// 		MainPage = new AppShell();
// 		        // MainPage = new NavigationPage(new LoginPage());

// 	}
// }
// 
// 
// 
// using Microsoft.Extensions.DependencyInjection;
// using ToolsManagerApp.Views;

// namespace ToolsManagerApp
// {
//     public partial class App : Application
//     {
//         public App(IServiceProvider serviceProvider)
//         {
//             InitializeComponent();

//             MainPage = new AppShell();

//             // Navigate to the LoginPage initially
//             // var shell = MainPage as AppShell;
//             // shell?.GoToAsync("//LoginPage");
// 			Shell.Current.GoToAsync("//LoginPage");

//         }
//     }
// }


// using MongoDB.Driver;
// using ToolsManagerApp.Repositories;

// namespace ToolsManagerApp
// {
//     public partial class App : Application
//     {
//         public static IUserRepository UserRepository { get; private set; }

//         public App()
//         {
//             InitializeComponent();

//             // Initialize MongoDB
//             var client = new MongoClient("mongodb+srv://booking:sDD0QQexwjmWbwXq@cluster0.xmntd0j.mongodb.net/");
//             var database = client.GetDatabase("ToolsManagerDb");

//             // Initialize UserRepository
//             UserRepository = new UserRepository(database);

//             MainPage = new AppShell();
//         }
//     }
// }
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToolsManagerApp.Repositories;
using ToolsManagerApp.ViewModels;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using DotNetEnv;

namespace ToolsManagerApp
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();
            Env.Load();


            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Ensure async call to SeedDatabase without blocking the main thread
            SeedDatabase().ConfigureAwait(false);

            MainPage = new AppShell();

            // Navigate to the LoginPage initially
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//LoginPage");
            });
        }

        private void ConfigureServices(IServiceCollection services)
        {

            // Get MongoDB connection string from environment variable
            var connectionString = Env.GetString("MONGODB_CONNECTION_STRING");

            // Initialize MongoDB
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("ToolsManagerDb");

            // Register logging
            services.AddLogging(configure => configure.AddConsole());

            // Register repositories
            services.AddSingleton<IUserRepository>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<UserRepository>>();
                return new UserRepository(database, logger);
            });

            // Register view models
            services.AddTransient<LoginViewModel>();
            services.AddTransient<UsersViewModel>();
        }

        private async Task SeedDatabase()
        {
            var userRepository = ServiceProvider.GetService<IUserRepository>();
            if (userRepository != null)
            {
                await userRepository.EnsureAdminUserAsync();
            }
        }
    }
}

