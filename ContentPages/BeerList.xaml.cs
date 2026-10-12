using MauiApp_StudyProject_.ViewModels;

namespace MauiApp_StudyProject_.ContentPages;

public partial class BeerList : ContentPage
{
    public BeerList(BeerViewModel viewModel)
	{
        InitializeComponent();

		BindingContext = viewModel;
	}
}