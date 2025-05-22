using KawaiiList.Models;
using Microsoft.Extensions.Options;
using Supabase;

namespace KawaiiList.Stores
{
    public class SupabaseClientStore
    {
        private readonly SupabaseSettings _settings;

        public Client Client { get; private set; }

        public SupabaseClientStore(IOptions<SupabaseSettings> options)
        {
            _settings = options.Value;
        }

        public async Task InitializeClientAsync()
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            Client = new Client(_settings.Url, _settings.AnonKey, options);
            await Client.InitializeAsync();
        }
    }
}
