using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp_StudyProject_.Models;

namespace MauiApp_StudyProject_.ViewModels
{
    [QueryProperty(nameof(Beer), nameof(Beer))]
    public partial class BeerDetailViewModel : BaseViewModel
    {
        [ObservableProperty]
        Beer beer;
    }
}
