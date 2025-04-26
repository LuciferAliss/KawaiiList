using KawaiiList.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace KawaiiList.Services
{
    public class AnilibriaService
    {
        private readonly HttpClient httpClient;

        private const string BaseUrl = "https://api.anilibria.tv/v3/";

        public AnilibriaService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(BaseUrl);
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<AnimeTitle>> SearchTitlesAsync(string query, CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"title/search?search={Uri.EscapeDataString(query)}", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AnimeTitle>>>(cancellationToken: token);

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

        public async Task<List<AnimeTitle>> GetTitlesAsync(int count, CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"title/updates?limit={count}", token);

                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AnimeTitle>>>(cancellationToken: token);

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


        public async Task<List<AnimeTitle>> GetUpdatesAsync(int count, CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"title/changes?limit={count}", token);

                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AnimeTitle>>>(cancellationToken: token);

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
    }
}