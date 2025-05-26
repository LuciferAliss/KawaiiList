using KawaiiList.Stores;
using Supabase;
using Supabase.Postgrest.Models;
using System.Diagnostics;
using static Supabase.Postgrest.Constants;

namespace KawaiiList.Services
{
    public class SupaBaseService<TTable> : ISupaBaseService<TTable>
        where TTable : BaseModel, new()
    {
        private readonly SupabaseClientStore _supabaseClientStore;

        private Client _client;

        public SupaBaseService(SupabaseClientStore supabaseClientStore)
        {
            _supabaseClientStore = supabaseClientStore;
            _client = _supabaseClientStore.Client;

            _supabaseClientStore.CurrentClientChanged += UpdateSession;
        }

        public async Task<IEnumerable<TTable>> GetFilter(string columns, string columnName, Operator op, object value)
        {
            try
            {
                var response = await _client.From<TTable>()
                    .Select(columns)
                    .Filter(columnName, op, value)
                    .Get();

                return response.Models;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в GetFilter: {ex.Message}");
                return [];
            }
        }

        public async Task<IEnumerable<TTable>> GetFilter(string columns, string columnName, Operator op, object value, string orderBy, Ordering or)
        {
            try
            {
                var response = await _client.From<TTable>()
                    .Select(columns)
                    .Filter(columnName, op, value)
                    .Order(orderBy, or)
                    .Get();

                return response.Models;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в GetFilter: {ex.Message}");
                return [];
            }
        }

        public async Task<bool> Insert(TTable value)
        {
            try
            {
                var response = await _client.From<TTable>().Insert(value);

                return response != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в Insert: {ex.Message}");
                return false;
            }
        }

        private void UpdateSession()
        {
            _client = _supabaseClientStore.Client;
        }
    }
}
