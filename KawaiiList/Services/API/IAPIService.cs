using KawaiiList.Models.Anilibria;

namespace KawaiiList.Services.API
{
    public interface IApiService
    {
        Task<List<AnimeTitle>> SearchTitlesAsync(string query, CancellationToken token);
        Task<List<AnimeTitle>> GetTitlesAsync(int count);
    }
}
