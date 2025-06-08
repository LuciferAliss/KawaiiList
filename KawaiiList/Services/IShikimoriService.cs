using KawaiiList.Models;

namespace KawaiiList.Services
{
    public interface IShikimoriService
    {
        Task<ShikimoriTitle> GetInfoAsync(string query, CancellationToken token);
        Task<List<ShikimoriTopic>> GetNewsAsync(CancellationToken token);
    }
}