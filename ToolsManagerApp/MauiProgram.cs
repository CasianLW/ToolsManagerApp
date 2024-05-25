using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ToolsManagerApp.Models;
using ToolsManagerApp.Repositories;
using ToolsManagerApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
namespace ToolsManagerApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
 // Register DbContext
            // builder.Services.AddDbContext<AppDbContext>();
		// 	builder.Services.AddDbContext<AppDbContext>(options =>
        // {
        //     options.UseSqlite($"Data Source={dbPath}");
        // });
		 // Add MongoDB settings
            builder.Services.Configure<MongoDBSettings>(options =>
            {
                options.ConnectionString = "your-mongodb-connection-string";
                options.DatabaseName = "toolsmanager";
            });

            // Register MongoDB client
            builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });
			// Register database
            builder.Services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(settings.DatabaseName);
            });

            // Register repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Register view models
            builder.Services.AddTransient<LoginViewModel>();
#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
