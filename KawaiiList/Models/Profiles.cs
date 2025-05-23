using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace KawaiiList.Models
{
    [Table("profiles")]
    public class Profiles : BaseModel
    {
        [PrimaryKey("id", true)]
        public string Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("nickname")]
        public string Nickname { get; set; }

        [Column("email")]
        public string Email { get; set; }
    }

    public class UserImages
    {
        public string AvatarUrl { get; set; }
        public string BannerUrl { get; set; }
    }
}