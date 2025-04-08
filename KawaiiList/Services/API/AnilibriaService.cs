using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using System.Buffers.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KawaiiList.Service.API
{
    public class AnilibriaService : IApiService
    {
        private readonly HttpClient httpClient;

        private const string BaseUrl = "https://api.anilibria.tv/v3/";

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

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
                var response = await httpClient.GetAsync($"title/search?query={Uri.EscapeDataString(query)}", token);

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

        public async Task<List<AnimeTitle>> GetTitlesAsync(int count)
        {
            try
            {
                var response = await httpClient.GetAsync($"title/updates?limit={count}");

                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AnimeTitle>>>();

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
    }
}
