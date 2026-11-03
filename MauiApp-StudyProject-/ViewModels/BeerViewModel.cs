using CommunityToolkit.Mvvm.Input;
using MauiApp_StudyProject_.Models;
using MauiApp_StudyProject_.Services;
using MauiApp_StudyProject_.Views;
using Microsoft.Extensions.Logging;
using SQLite;
using System.Collections.ObjectModel;

namespace MauiApp_StudyProject_.ViewModels
{
    public partial class BeerViewModel : BaseViewModel
    {
        private readonly IBeerService _beerService;
        private readonly ILogger<BeerViewModel> _logger;

        public ObservableCollection<Beer> Beers { get; } = new();

        public BeerViewModel(
            IBeerService beerService,
            ILogger<BeerViewModel> logger)
        {
            Title = "Beer List";

            _beerService = beerService;
            _logger = logger;
        }

        [RelayCommand]
        private async Task LoadBeersAsync(CancellationToken cancellationToken = default)
        {
            if (IsLoading)
            {
                IsRefreshing = false;
                return;
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                IsLoading = true;
                IsRefreshing = true;

                var beers = await _beerService.GetBeersAsync();

                Beers.Clear();

                foreach (var beer in beers)
                {
                    Beers.Add(beer);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Loading beers operation was canceled.");
            }
            catch (SQLiteException sqlEx)
            {
                _logger.LogError(sqlEx, "SQLite error while loading beers");

                await Shell.Current.DisplayAlertAsync(
                    "Database Error",
                    "An error occurred while accessing the database. Please try again later.",
                    "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading beers");

                await Shell.Current.DisplayAlertAsync(
                    "Error",
                    "Failed to load beers. Please try again later.",
                    "OK");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task ViewBeerAsync(Beer beer)
        {
            _logger.LogInformation("Beer selected: {BeerName}({BeerId})", beer.Name, beer.Id);

            // Navigate to the BeerDetailPage and pass the selected beer as a parameter
            await Shell.Current.GoToAsync($"{nameof(BeerDetailPage)}?mode={BeerPageMode.View}&beerId={beer.Id}");
        }

        [RelayCommand]
        private async Task AddBeerAsync()
        {
            _logger.LogInformation("Navigating to BeerDetailPage in Create mode.");

            // Navigate to the BeerDetailPage in Create mode
            await Shell.Current.GoToAsync($"{nameof(BeerDetailPage)}?mode={BeerPageMode.Create}");
        }

        [RelayCommand]
        private async Task EditBeerAsync(Beer beer)
        {
            _logger.LogInformation("Navigating to BeerDetailPage in Edit mode. Beer: {BeerName}({BeerId})", beer.Name, beer.Id);
            
            // Navigate to the BeerDetailPage in Edit mode
            await Shell.Current.GoToAsync($"{nameof(BeerDetailPage)}?mode={BeerPageMode.Edit}&beerId={beer.Id}");
        }

        [RelayCommand]
        private async Task DeleteBeerAsync(Beer beer)
        {
            _logger.LogInformation("Attempting to delete beer: {BeerName}({BeerId})", beer.Name, beer.Id);

            if (beer == null)
                return;

            bool confirmDelete = await Shell.Current.DisplayAlertAsync(
                "Confirm Delete",
                $"Are you sure you want to delete '{beer.Name}'?",
                "Yes",
                "No");

            if (!confirmDelete)
                return;

            try
            {
                await _beerService.DeleteBeerAsync(beer);

                Beers.Remove(beer);

                _logger.LogInformation("Deleted beer: {BeerName}", beer.Name);
            }
            catch (SQLiteException sqlEx)
            {
                _logger.LogError(sqlEx, "SQLite error while deleting beer");

                await Shell.Current.DisplayAlertAsync(
                    "Database Error",
                    "An error occurred while accessing the database. Please try again later.",
                    "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting beer");

                await Shell.Current.DisplayAlertAsync(
                    "Error",
                    "Failed to delete the beer. Please try again later.",
                    "OK");
            }
        }
    }
}