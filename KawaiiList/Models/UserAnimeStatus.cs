using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace KawaiiList.Models
{
    [Table("user_anime_status")]
    public class UserAnimeStatus : BaseModel
    {
        [PrimaryKey("id", true)]
        public string Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("anime_id")]
        public int AnimeId { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("score")]
        public int? Score { get; set; }

        [Column("progress")]
        public int? Progress { get; set; }
    }
}
