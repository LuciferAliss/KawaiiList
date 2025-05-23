﻿namespace KawaiiList.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
    }

    public class UserImages
    {
        public string AvatarUrl { get; set; }
        public string BannerUrl { get; set; }
    }
}