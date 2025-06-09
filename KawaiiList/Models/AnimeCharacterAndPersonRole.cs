using System.Text.Json.Serialization;

namespace KawaiiList.Models
{
    public class AnimeCharacterAndPersonRole
    {
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }

        [JsonPropertyName("roles_russian")]
        public List<string> RolesRussian { get; set; }

        [JsonPropertyName("character")]
        public Character Character { get; set; }

        [JsonPropertyName("person")]
        public ShikimoriPerson Person { get; set; }
    
        public string? Role => string.Join(", ", Roles);
    }

    public class Character
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("russian")]
        public string Russian { get; set; }

        [JsonPropertyName("image")]
        public ImageInfo Image { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class Person
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("russian")]
        public string RussianName { get; set; }
    }

    public class ShikimoriPerson
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("russian")]
        public string Russian { get; set; }

        [JsonPropertyName("image")]
        public ImageInfo Image { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class ImageInfo
    {
        
        [JsonPropertyName("original")]
        private string? _original;

        public string? Original
        {
            get => _original;
            set => _original = value != null ? "https://shikimori.one" + value : null;
        }
    }
}
