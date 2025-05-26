using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using static Supabase.Postgrest.Constants;

namespace KawaiiList.Services
{
    public interface ISupaBaseService<TTable> where TTable : BaseModel, new()
    {
        Task<IEnumerable<TTable>> GetFilter(string columns, string columnName, Constants.Operator op, object value);
        Task<IEnumerable<TTable>> GetFilter(string columns, string columnName, Operator op, object value, string orderBy, Ordering or);
        Task<bool> Insert(TTable value);
    }
}