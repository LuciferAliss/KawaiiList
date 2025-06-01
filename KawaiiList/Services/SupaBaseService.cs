using KawaiiList.Models;
using KawaiiList.Stores;
using Supabase;
using Supabase.Postgrest.Models;
using System.Diagnostics;
using System.Linq.Expressions;
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

        public async Task<IEnumerable<TTable>> GetFilter(
            string columns,
            FiltersQuery filter)
        {
            try
            {
                var query = _client.From<TTable>().Select(columns);

                var (columnName, operatorFilter, value) = filter;
                query = query.Filter(columnName, operatorFilter, value);

                var response = await query.Get();
                return response.Models;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в GetFilter: {ex.Message}");
                return [];
            }
        }

        public async Task<IEnumerable<TTable>> GetFilter(
            string columns,
            List<FiltersQuery> filters)
        {
            try
            {
                var query = _client.From<TTable>().Select(columns);

                foreach (var (columnName, op, value) in filters)
                {
                    query = query.Filter(columnName, op, value);
                }

                var response = await query.Get();
                return response.Models;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в GetFilters: {ex.Message}");
                return [];
            }
        }

        public async Task<IEnumerable<TTable>> GetFilter(
            string columns,
            List<FiltersQuery> filters,
            string orderBy,
            Ordering or)
        {
            try
            {
                var query = _client.From<TTable>().Select(columns);

                foreach (var (columnName, op, value) in filters)
                {
                    query = query.Filter(columnName, op, value);
                }

                var response = await query.Order(orderBy, or).Get();
                return response.Models;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в GetFilters: {ex.Message}");
                return [];
            }
        }

        public async Task<bool> Upsert(TTable value)
        {
            try
            {
                var response = await _client
                    .From<TTable>()
                    .Upsert(value);

                return response != null && response.Models != null && response.Models.Any();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в Upsert: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Upsert(TTable value, string conflict)
        {
            try
            {
                var response = await _client
                    .From<TTable>()
                    .OnConflict(conflict)
                    .Upsert(value);

                return response != null && response.Models != null && response.Models.Any();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в Upsert: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(Expression<Func<TTable, bool>> predicate)
        {
            try
            {
                await _client
                    .From<TTable>()
                    .Where(predicate)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в Delete: {ex.Message}");
                return false;
            }
        }

        private void UpdateSession()
        {
            _client = _supabaseClientStore.Client;
        }
    }
}
