using KawaiiList.Models;

namespace KawaiiList.Services
{
    public interface IAnilibriaService
    {
        Task<List<AnilibriaTitle>> GetTitlesAsync(int count, CancellationToken token);
        Task<List<AnilibriaTitle>> SearchTitlesAsync(string query, CancellationToken token);
        Task<List<string>> GetGenresAsync(CancellationToken token);
        Task<List<int>> GetYearsAsync(CancellationToken token);
        Task<List<AnilibriaTitle>> GetPageAsync(string genre, int? year, CancellationToken token);
    }
}