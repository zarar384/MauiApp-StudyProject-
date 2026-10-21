namespace MauiApp_StudyProject_;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(Views.BeerDetailPage), typeof(Views.BeerDetailPage));
    }
}
