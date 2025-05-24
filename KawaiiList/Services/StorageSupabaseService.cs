using Supabase;

namespace KawaiiList.Services
{
    public class StorageSupabaseService : IStorageSupabaseService
    {
        private Client _clinet;

        public async Task GetImage()
        {
            var response = await _clinet.Storage.From("avatars").DownloadPublicFile("");
        }
    }
}
