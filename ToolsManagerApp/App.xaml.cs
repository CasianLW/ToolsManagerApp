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
using ToolsManagerApp.Models;
using System.Linq;
using System.Collections.Generic;

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
            var connectionDatabase = Env.GetString("MONGO_DATABASE_NAME");

            // Initialize MongoDB
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(connectionDatabase);

            // Register MongoDB instance
            services.AddSingleton(database);

            // Register logging
            services.AddLogging(configure => configure.AddConsole());

            // Register repositories
            services.AddSingleton<IUserRepository>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<UserRepository>>();
                return new UserRepository(database, logger);
            });

            services.AddSingleton<IToolRepository>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ToolRepository>>();
                return new ToolRepository(database, logger);
            });

            services.AddSingleton<ICategoryRepository>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<CategoryRepository>>();
                return new CategoryRepository(database, logger);
            });

            // Register view models
            services.AddTransient<LoginViewModel>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<ToolsViewModel>();
            services.AddTransient<UserToolsViewModel>();
        }

        private async Task SeedDatabase()
        {
            var userRepository = ServiceProvider.GetService<IUserRepository>();
            if (userRepository != null)
            {
                await userRepository.EnsureAdminUserAsync();
            }

            var toolRepository = ServiceProvider.GetService<IToolRepository>();
            var categoryRepository = ServiceProvider.GetService<ICategoryRepository>();
            if (toolRepository != null && categoryRepository != null)
            {
                await EnsureCategoriesAndToolsAsync(toolRepository, categoryRepository);
            }
        }

        private async Task EnsureCategoriesAndToolsAsync(IToolRepository toolRepository, ICategoryRepository categoryRepository)
        {
            // Seed categories
            var existingCategories = await categoryRepository.GetAllCategoriesAsync();
            if (existingCategories == null || !existingCategories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Hand Tools", Description = "Manual tools" },
                    new Category { Name = "Power Tools", Description = "Tools powered by electricity" },
                    new Category { Name = "Garden Tools", Description = "Tools for gardening" }
                };

                foreach (var category in categories)
                {
                    await categoryRepository.AddCategoryAsync(category);
                }
            }

            // Seed tools
            var existingTools = await toolRepository.GetAllToolsAsync();
            if (existingTools == null || !existingTools.Any())
            {
                var categories = await categoryRepository.GetAllCategoriesAsync();

                var tools = new List<Tool>
                {
                    new Tool { Name = "Hammer", CategoryId = categories.First(c => c.Name == "Hand Tools").Id, Status = StatusEnum.Available, QRCode = "12345" },
                    new Tool { Name = "Drill", CategoryId = categories.First(c => c.Name == "Power Tools").Id, Status = StatusEnum.Available, QRCode = "67890" },
                    new Tool { Name = "Rake", CategoryId = categories.First(c => c.Name == "Garden Tools").Id, Status = StatusEnum.Available, QRCode = "11223" }
                };

                foreach (var tool in tools)
                {
                    await toolRepository.AddToolAsync(tool);
                }
            }
        }
    }
}
