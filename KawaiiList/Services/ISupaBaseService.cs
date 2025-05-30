using KawaiiList.Models;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using System.Linq.Expressions;
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
            List<FiltersQuery> filters);

        Task<IEnumerable<TTable>> GetFilter(
            string columns,
            List<FiltersQuery> filters,
            string orderBy,
            Ordering or);

        Task<bool> Upsert(TTable value);
        Task<bool> Upsert(TTable value, string conflict);
        Task<bool> Delete(Expression<Func<TTable, bool>> predicate);
    }
}