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
        private readonly BeerService _beerService;
        private readonly ILogger<BeerViewModel> _logger;

        public ObservableCollection<Beer> Beers { get; } = new();

        public BeerViewModel(
            BeerService beerService,
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
        private async Task BeerSelected(Beer beer)
        {
            _logger.LogInformation("Beer selected: {BeerName}", beer.Name);

            if(beer == null)
                return;

            // Navigate to the BeerDetailPage and pass the selected beer as a parameter
            await Shell.Current.GoToAsync($"{nameof(BeerDetailPage)}", new Dictionary<string, object>
            {
                { nameof(Beer), beer }
            });
        }
    }
}