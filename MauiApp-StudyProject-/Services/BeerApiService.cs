
using MauiApp_StudyProject_.Configuration;
using MauiApp_StudyProject_.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MauiApp_StudyProject_.Services
{
    public class BeerApiService : IBeerApiService
    {
        private const string Endpoint = "api/beers";
        private readonly HttpClient _http;

        public BeerApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Beer>> GetBeersAsync()
        {
            using var response = await _http.GetAsync(Endpoint);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<Beer>>()
                ?? throw new JsonException("Failed to deserialize the list of beers.");
        }

        public async Task<Beer?> GetBeerByIdAsync(int beerId)
        {
            using var response = await _http.GetAsync($"{Endpoint}/{beerId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<Beer>()
                ?? throw new JsonException("Failed to deserialize the beer.");
        }

        public async Task<Beer> AddBeerAsync(Beer beer)
        {
            using var response = await _http.PostAsJsonAsync(Endpoint, beer);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<Beer>()
                ?? throw new JsonException("Failed to deserialize the added beer.");
        }

        public async Task UpdateBeerAsync(int beerId, Beer beer)
        {
            using var response = await _http.PutAsJsonAsync($"{Endpoint}/{beerId}", beer);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBeerAsync(int beerId)
        {
            using var response = await _http.DeleteAsync($"{Endpoint}/{beerId}");

            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if the API is available by sending a HEAD request
                // to the base URL with a timeout of 2 seconds.
                using var timeout = CancellationTokenSource
                    .CreateLinkedTokenSource(cancellationToken);

                timeout.CancelAfter(TimeSpan.FromSeconds(2));

                using var response = await _http.GetAsync(
                    "api/beers/health",
                    HttpCompletionOption.ResponseHeadersRead,
                    timeout.Token);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (OperationCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return false;
            }
        }
    }
}
