using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp_StudyProject_.Models;
using MauiApp_StudyProject_.Services;
using Microsoft.Extensions.Logging;

namespace MauiApp_StudyProject_.ViewModels
{
    public enum BeerPageMode
    {
        Create,
        Edit,
        View
    }

    //[QueryProperty(nameof(Beer), nameof(Beer))]
    public partial class BeerDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IBeerService _beerService;
        private readonly ILogger<BeerDetailViewModel> _logger;

        [ObservableProperty]
        private Beer? beer;

        [ObservableProperty]
        private int? beerId;

        [ObservableProperty]
        private BeerPageMode mode;

        public bool IsReadOnly => Mode == BeerPageMode.View;

        public bool IsEditMode => Mode == BeerPageMode.Edit;

        public bool IsSaveVisible => 
            Mode is BeerPageMode.Create or BeerPageMode.Edit;

        public string PageTitle => Mode switch
        {
            BeerPageMode.Create => "Add New Beer",
            BeerPageMode.Edit => "Edit Beer",
            BeerPageMode.View => "Beer Details",
            _ => throw new InvalidOperationException("Invalid mode")
        };

        private bool _isInitialized;

        public BeerDetailViewModel(
            IBeerService beerService, 
            ILogger<BeerDetailViewModel> logger)
        {
            _beerService = beerService;
            _logger = logger;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _logger.LogInformation("Applying query attributes: {Query}", query);

            Mode = GetMode(query);
            BeerId = GetBeerId(query);

            _isInitialized = false;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing BeerDetailViewModel with Mode: {Mode}, BeerId: {BeerId}", Mode, BeerId);

            if (_isInitialized)
            {
                return;
            }
            
            if (Mode == BeerPageMode.Create)
            {
                Beer = new Beer();
                _isInitialized = true;
                return;
            }

            if(BeerId is null)
            {
                throw new InvalidOperationException("BeerId must be provided for Edit or View mode.");
            }

            Beer = await _beerService.GetBeerByIdAsync(BeerId.Value);

            if(Beer is null)
            {
                throw new InvalidOperationException($"No beer found with Id {BeerId.Value}");
            }

            _isInitialized = true;
        }    

        private static BeerPageMode GetMode(IDictionary<string, object> query)
        {
            if (query.TryGetValue("mode", out var modeObj) &&
                modeObj is string modeStr &&
                Enum.TryParse<BeerPageMode>(modeStr, ignoreCase: true, out var mode))
            {
                return mode;
            }

            return BeerPageMode.View; // Default to View if not specified
        }

        [RelayCommand]
        private async Task SaveBeerAsync()
        {
            _logger.LogInformation("Saving beer. Mode: {Mode}, Beer: {@Beer}", Mode, Beer);

            if (Beer is null)
            {
                throw new InvalidOperationException("Beer cannot be null when saving.");
            }

            IsLoading = true;

            try
            {
                if (Mode == BeerPageMode.Create)
                {
                    await _beerService.AddBeerAsync(Beer);
                }
                else if (Mode == BeerPageMode.Edit)
                {
                    await _beerService.UpdateBeerAsync(Beer);
                }

                await Shell.Current.GoToAsync(".."); // Navigate back after saving
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the beer. Mode: {Mode}, Beer: {@Beer}", Mode, Beer);

                await Shell.Current.DisplayAlertAsync(
                    "Save Error",
                    "Failed to save the beer. Please try again.",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static int? GetBeerId(IDictionary<string, object> query)
        {
            if (query.TryGetValue("beerId", out var beerIdObj) &&
                beerIdObj is string beerIdStr &&
                int.TryParse(beerIdStr, out var beerId))
            {
                return beerId;
            }

            return null; // Return null if not specified or invalid
        }

        partial void OnModeChanged(BeerPageMode value)
        {
            // Notify that IsReadOnly and IsSaveVisible properties may have changed
            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(IsSaveVisible));
            OnPropertyChanged(nameof(PageTitle));
        }
    }
}
