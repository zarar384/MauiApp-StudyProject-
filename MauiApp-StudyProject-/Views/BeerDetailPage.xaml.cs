using MauiApp_StudyProject_.ViewModels;

namespace MauiApp_StudyProject_.Views;

public partial class BeerDetailPage : ContentPage
{
    private readonly BeerDetailViewModel _viewModel;

    public BeerDetailPage(BeerDetailViewModel viewModel)
	{
		InitializeComponent();

        _viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.InitializeAsync();
    }
}