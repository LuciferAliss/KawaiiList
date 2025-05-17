using KawaiiList.Models;

namespace KawaiiList.Services
{
    public interface IAnilibriaService
    {
        Task<List<AnimeTitle>> GetTitlesAsync(int count, CancellationToken token);
        Task<List<AnimeTitle>> SearchTitlesAsync(string query, CancellationToken token);
    }
}