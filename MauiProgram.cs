using MauiApp_StudyProject_.Data;
using MauiApp_StudyProject_.Services;
using MauiApp_StudyProject_.ViewModels;
using Microsoft.Extensions.Logging;
using SQLite;

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

        // Register services
        builder.Services.AddSingleton<IBeerService, BeerService>();

        // Register view models
        builder.Services.AddSingleton<BeerViewModel>();
		builder.Services.AddSingleton<BeerDetailViewModel>();

        // Register pages
        builder.Services.AddSingleton<ContentPages.BeerList>();
		builder.Services.AddSingleton<Views.BeerDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
