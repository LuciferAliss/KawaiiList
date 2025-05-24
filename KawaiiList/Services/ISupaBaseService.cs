using Supabase.Postgrest;
using Supabase.Postgrest.Models;

namespace KawaiiList.Services
{
    public interface ISupaBaseService<TTable> where TTable : BaseModel, new()
    {
        Task<IEnumerable<TTable>> GetFilter(string columns, string columnName, Constants.Operator op, object value);
        Task<bool> Insert(TTable value);
    }
}