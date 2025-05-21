using KawaiiList.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace KawaiiList.Services
{
    public class AnilibriaService : IAnilibriaService
    {
        private readonly HttpClient httpClient;

        private const string BaseUrl = "https://api.anilibria.tv/v3/";

        public AnilibriaService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(BaseUrl);
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<AnilibriaTitle>> SearchTitlesAsync(string query, CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"title/search?search={Uri.EscapeDataString(query)}", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<AnilibriaTitles>(cancellationToken: token);

                return result?.List ?? [];
            }
            catch (OperationCanceledException)
            {
                return [];
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP request error: {httpEx.Message}");
                return [];
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON processing error: {jsonEx.Message}");
                return [];
            }
        }

        public async Task<List<AnilibriaTitle>> GetTitlesAsync(int count, CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"title/updates?limit={count}", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<AnilibriaTitles>(cancellationToken: token);

                return result?.List ?? [];
            }
            catch (OperationCanceledException)
            {
                return [];
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP request error: {httpEx.Message}");
                return [];
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"JSON processing error: {jsonEx.Message}");
                return [];
            }
        }

        public async Task<(List<AnilibriaTitle>, bool)> GetPageTitlesAsync(int page, CancellationToken token)
        {
            while (true)
            {
                try
                {
                    var response = await httpClient.GetAsync($"title/updates?page={page}&items_per_page=21", cancellationToken: token);

                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadFromJsonAsync<AnilibriaTitles>(cancellationToken: token);

                    if (result?.List?.Count == 0 || result?.List == null)
                    {
                        return (result?.List ?? [], false);
                    }

                    return (result?.List ?? [], true);
                }
                catch (OperationCanceledException)
                {
                }
                catch (HttpRequestException httpEx)
                {
                    Debug.WriteLine($"HTTP request error: {httpEx.Message}");
                }
                catch (JsonException jsonEx)
                {
                    Debug.WriteLine($"JSON processing error: {jsonEx.Message}");
                }
            }
        }

        public async Task<(List<AnilibriaTitle>, bool)> GetSortTitlesAsync(int page, string genre, int? year, CancellationToken token)
        {
            string uri;
            const int Limit = 21;
            var qp = new List<string>
            {
                $"page={page}",
                $"items_per_page={Limit}"
            };

            qp.Add("filter=");

            if (genre != "Любой")
            {
                qp.Add($"genres={Uri.EscapeDataString(genre)},");
            }

            if (year.HasValue)
            {
                qp.Add($"year={year.Value},");
            }

            uri = $"title/search?{string.Join("&", qp)}";

            while (true)
            {
                try
                {
                    var response = await httpClient.GetAsync(uri, cancellationToken: token);

                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadFromJsonAsync<AnilibriaTitles>(cancellationToken: token);

                    if (result?.List?.Count == 0 || result?.List == null)
                    {
                        return (result?.List ?? [], false);
                    }

                    return (result?.List ?? [], true);
                }
                catch (OperationCanceledException)
                {
                }
                catch (HttpRequestException httpEx)
                {
                    Debug.WriteLine($"HTTP request error: {httpEx.Message}");
                }
                catch (JsonException jsonEx)
                {
                    Debug.WriteLine($"JSON processing error: {jsonEx.Message}");
                }
            }
        }

        public async Task<List<ScheduleAnilibriaTitles>> GetScheduleAsync(CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"title/schedule", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<ScheduleAnilibriaTitles>>(cancellationToken: token);

                return result ?? [];
            }
            catch (OperationCanceledException)
            {
                return [];
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP request error: {httpEx.Message}");
                return [];
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"JSON processing error: {jsonEx.Message}");
                return [];
            }
        }

        public async Task<List<string>> GetGenresAsync(CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"genres", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<string>> (cancellationToken: token);

                return result ?? [];
            }
            catch (OperationCanceledException)
            {
                return [];
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP request error: {httpEx.Message}");
                return [];
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"JSON processing error: {jsonEx.Message}");
                return [];
            }
        }

        public async Task<List<int>> GetYearsAsync(CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"years", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<int>>(cancellationToken: token);

                return result ?? [];
            }
            catch (OperationCanceledException)
            {
                return [];
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP request error: {httpEx.Message}");
                return [];
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"JSON processing error: {jsonEx.Message}");
                return [];
            }
        }

        public async Task<AnilibriaTitle> GetRandomAsync(CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"title/random", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<AnilibriaTitle>(cancellationToken: token);

                return result ?? new();
            }
            catch (OperationCanceledException)
            {
                return new();
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP request error: {httpEx.Message}");
                return new();
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"JSON processing error: {jsonEx.Message}");
                return new();
            }
        }
    }
}