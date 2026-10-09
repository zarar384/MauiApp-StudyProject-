using CommunityToolkit.Mvvm.Input;
using MauiApp_StudyProject_.Models;
using MauiApp_StudyProject_.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MauiApp_StudyProject_.ViewModels
{
    public partial class BeerViewModel : BaseViewModel
    {
        private readonly BeerService _beerService;
        private readonly ILogger<BeerViewModel> _logger;
        public ObservableCollection<Beer> Beers { get; private set; }

        public BeerViewModel(BeerService beerService, ILogger<BeerViewModel> logger)
        {
            Title = "Beer List";

            _beerService = beerService;
            _logger = logger;
        }

        [RelayCommand]
        async Task LoadBeersAsync(CancellationToken cancellationToken = default)
        {
            if (IsLoading)
                return;

            try
            {
                IsLoading = true;

                var beers = await _beerService.GetBeersAsync(cancellationToken);

                Beers = new ObservableCollection<Beer>(beers);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Loading beers operation was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading beers");

                await Shell.Current.DisplayAlertAsync("Error", "Failed to load beers. Please try again later.", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
