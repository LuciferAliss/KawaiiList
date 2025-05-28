using static Supabase.Postgrest.Constants;

namespace KawaiiList.Models
{
    public class FiltersQuery
    {
        public string ColumnName { get; set; }
        public Operator OperatorFilter { get; set; }
        public object Value { get; set; }

        internal void Deconstruct(out string columnName, out Operator operatorFilter, out object value)
        {
            columnName = ColumnName;
            operatorFilter = OperatorFilter;
            value = Value;
        }
    }
}
