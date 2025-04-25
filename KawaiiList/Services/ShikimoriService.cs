using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using KawaiiList.Models;

namespace KawaiiList.Services
{
    public class ShikimoriService
    {
        private readonly HttpClient httpClient;

        private const string BaseUrl = "https://shikimori.one/api/";

        public ShikimoriService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(BaseUrl);
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

                List<AnimeRole>? roles = await response.Content.ReadFromJsonAsync<List<AnimeRole>>(cancellationToken: token);

                // Находим оригинального автора (может быть несколько)
                List<AnimeRole>? originalAuthors = roles?
                    .Where(r => r.Roles.Any(role =>
                        role.Contains("Original Creator") ||
                        role.Contains("Автор оригинала") ||
                        role.Contains("Оригинальный автор")
                    ))
                    .ToList();

                resultInfo.AuthorInfo = originalAuthors?.FirstOrDefault();

                return resultInfo ?? new ShikimoriTitle();
            }
            catch (OperationCanceledException)
            {
                return new ShikimoriTitle();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP request error: {httpEx.Message}");
                return new ShikimoriTitle();
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON processing error: {jsonEx.Message}");
                return new ShikimoriTitle();
            }
        }
    }
}
