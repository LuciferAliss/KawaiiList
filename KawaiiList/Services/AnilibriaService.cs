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

                string responseBody = await response.Content.ReadAsStringAsync();
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

        public async Task<List<AnilibriaTitle>> GetPageAsync(string genre, int? year, CancellationToken token)
        {
            string uri;
            const int Limit = 2000;
            var qp = new List<string>
            {
                $"limit={Limit}",
            };

            if (genre != "Любой" || year.HasValue)
            {
                qp.Add("filter=");

                if (genre != "Любой")
                {
                    qp.Add($"genres={Uri.EscapeDataString(genre)},");
                }

                if (year.HasValue)
                {
                    qp.Add($"year={year.Value},");
                }

                uri = $"v3/title/search?{string.Join("&", qp)}";
            }
            else
            {
                uri = $"title/updates?{string.Join("&", qp)}";
            }

            try
            {
                var response = await httpClient.GetAsync(uri, token);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
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

        public async Task<List<string>> GetGenresAsync(CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"genres", token);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
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

                string responseBody = await response.Content.ReadAsStringAsync();
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
    }
}