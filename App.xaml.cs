using MauiApp_StudyProject_.Data;
using Microsoft.Extensions.DependencyInjection;

namespace MauiApp_StudyProject_;

public partial class App : Application
{
	private readonly AppDatabase _db;

	public App(AppDatabase db)
	{
		InitializeComponent();
		
		_db = db;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var loadingPage = new ContentPage
		{
			Content = new ActivityIndicator
			{
				IsRunning = true,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			}
		};

		var window = new Window(loadingPage);

		loadingPage.Loaded += async (_, _) =>
		{
			try
			{
				await _db.InitializeAsync();
				window.Page = new AppShell();
            }
			catch(Exception ex)
			{
				loadingPage.Content = new Label
                {
                    Text = $"Failed to initialize the database: {ex.Message}",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
            }
		};

		return window;
    }
}