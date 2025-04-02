using KawaiiList.Models;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;

namespace KawaiiList.Service.API
{
    class APIManager
    {
        private readonly static HttpClient client = new()
        {
            BaseAddress = new Uri("https://api.anilibria.tv/v3/")
        };

        public static async Task<List<Anime>?> SearchAnime(string name, CancellationToken token)
        {
            try
            {
                var responseMessage = await client.GetAsync($"title/search?search={Uri.EscapeDataString(name)}", token);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка: {responseMessage.StatusCode} - {responseMessage.ReasonPhrase}");
                    return null;
                }

                var content = await responseMessage.Content.ReadAsStringAsync(token);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var animeResponse = JsonSerializer.Deserialize<AnimeResponse>(content, options);

                return animeResponse?.List;
            }
            catch (OperationCanceledException)
            {
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Ошибка HTTP запроса: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Ошибка при обработке JSON: {jsonEx.Message}");
            }

            return null;
        }
    }
}
