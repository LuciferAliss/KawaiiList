using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace KawaiiList.Models
{
    [Table("user_image")]
    public class UserImage : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("type_image")]
        public string TypeImage { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("uploaded_at")]
        public DateTime UploadedAt { get; set; }
    }
}
