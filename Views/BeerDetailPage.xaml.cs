using MauiApp_StudyProject_.ViewModels;

namespace MauiApp_StudyProject_.Views;

public partial class BeerDetailPage : ContentPage
{
	public BeerDetailPage(BeerDetailViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}