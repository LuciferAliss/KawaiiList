using KawaiiList.Models;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using static Supabase.Postgrest.Constants;

namespace KawaiiList.Services
{
    public interface ISupaBaseService<TTable> where TTable : BaseModel, new()
    {
        Task<IEnumerable<TTable>> GetFilter(
            string columns,
            FiltersQuery filter);

        Task<IEnumerable<TTable>> GetFilter(
            string columns,
            List<FiltersQuery> filters,
            string orderBy,
            Ordering or);

            Task<bool> Insert(TTable value);
    }
}