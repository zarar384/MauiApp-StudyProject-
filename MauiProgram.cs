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

        // Register services
        builder.Services.AddSingleton<BeerService>();

        // Register view models
        builder.Services.AddSingleton<BeerViewModel>();

        // Register pages
        builder.Services.AddSingleton<ContentPages.BeerList>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
