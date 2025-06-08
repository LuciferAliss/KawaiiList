using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using KawaiiList.Models;
using System.Diagnostics;

namespace KawaiiList.Services
{
    public class ShikimoriService : IShikimoriService
    {
        private readonly HttpClient httpClient;

        private const string BaseUrl = "https://shikimori.one/api/";

        public ShikimoriService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(BaseUrl);
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<ShikimoriTopic>> GetNewsAsync(CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"topics?forum=news&limit=3", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<ShikimoriTopic>>(cancellationToken: token);

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

        public async Task<ShikimoriTitle> GetInfoAsync(string query, CancellationToken token)
        {
            try
            {
                var response = await httpClient.GetAsync($"animes?search={Uri.EscapeDataString(query)}", token);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<ShikimoriTitle>>(cancellationToken: token);

                response = await httpClient.GetAsync($"{result[0].Url.Substring(1)}", token);

                var resultInfo = await response.Content.ReadFromJsonAsync<ShikimoriTitle>(cancellationToken: token);

                response = await httpClient.GetAsync($"animes/{resultInfo.Id}/roles", token);

                List<AnimeCharacterAndPersonRole>? roles = await response.Content.ReadFromJsonAsync<List<AnimeCharacterAndPersonRole>>(cancellationToken: token);

                resultInfo.AuthorAndCharacterInfo = roles;

                return resultInfo ?? new ShikimoriTitle { Id = -1 };
            }
            catch (OperationCanceledException)
            {
                return new ShikimoriTitle { Id = -1 };
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP request error: {httpEx.Message}");
                return new ShikimoriTitle { Id = -1 };
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON processing error: {jsonEx.Message}");
                return new ShikimoriTitle { Id = -1 };
            }
        }
    }
}