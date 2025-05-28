using System.Security.Policy;

namespace KawaiiList.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public UserImageProfil Images { get; set; }
    }

    public class UserImageProfil
    {
        public string _avatarUrl;

        public string? AvatarUrl
        {
            get => _avatarUrl;
            set => _avatarUrl = value != null ? "https://uhzentqgqhjoiasledqe.supabase.co/storage/v1/object/public/" + value : "";
        }

        public string _bannerUrl;
        public string BannerUrl
        {
            get => _bannerUrl;
            set => _bannerUrl = value != null ? "https://uhzentqgqhjoiasledqe.supabase.co/storage/v1/object/public/" + value : "";
        }
    }
}