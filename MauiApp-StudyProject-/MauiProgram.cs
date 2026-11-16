using MauiApp_StudyProject_.Configuration;
using MauiApp_StudyProject_.Data;
using MauiApp_StudyProject_.Services;
using MauiApp_StudyProject_.ViewModels;
using Microsoft.Extensions.Logging;

namespace MauiApp_StudyProject_;

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

		// Register SQLite connection
		builder.Services.AddSingleton<AppDatabase>();
        builder.Services.AddSingleton<IBeerService, BeerService>();

        // HTTP API service
        builder.Services.AddHttpClient<IBeerApiService, BeerApiService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        });

        // Service selector to choose between API and local database
        builder.Services.AddTransient<IBeerSelectorService, BeerServiceSelector>();

        // Register view models
        builder.Services.AddTransient<BeerViewModel>();
		builder.Services.AddTransient<BeerDetailViewModel>();

        // Register pages
        builder.Services.AddTransient<ContentPages.BeerList>();
		builder.Services.AddTransient<Views.BeerDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
