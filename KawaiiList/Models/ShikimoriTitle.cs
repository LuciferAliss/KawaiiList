using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KawaiiList.Models
{
    public class ShikimoriTitle
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("aired_on")]
        public string? DateStart { get; set; }

        [JsonPropertyName("released_on")]
        public string? DateEnd { get; set; }

        [JsonPropertyName("image")]
        public ImageData? Image { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("score")]
        public string? Score { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("rating")]
        public string? Rating { get; set; }

        [JsonPropertyName("english")]
        public List<string>? EnglishTitles { get; set; }

        [JsonPropertyName("japanese")]
        public List<string>? JapaneseTitles { get; set; }

        [JsonPropertyName("synonyms")]
        public List<string>? Synonyms { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("description_html")]
        public string? DescriptionHtml { get; set; }

        [JsonPropertyName("description_source")]
        public string? DescriptionSource { get; set; }

        [JsonPropertyName("franchise")]
        public string? Franchise { get; set; }

        [JsonPropertyName("anons")]
        public bool Anons { get; set; }

        [JsonPropertyName("ongoing")]
        public bool Ongoing { get; set; }

        [JsonPropertyName("thread_id")]
        public int ThreadId { get; set; }

        [JsonPropertyName("topic_id")]
        public int TopicId { get; set; }

        [JsonPropertyName("rates_scores_stats")]
        public List<RateScoreStat>? RatesScoresStats { get; set; }

        [JsonPropertyName("rates_statuses_stats")]
        public List<RateStatusStat>? RatesStatusesStats { get; set; }

        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("next_episode_at")]
        public string? NextEpisodeAt { get; set; }

        [JsonPropertyName("fansubbers")]
        public List<string>? Fansubbers { get; set; }

        [JsonPropertyName("fandubbers")]
        public List<string>? Fandubbers { get; set; }

        [JsonPropertyName("licensors")]
        public List<string>? Licensors { get; set; }

        [JsonPropertyName("videos")]
        public List<Video>? Videos { get; set; }

        [JsonPropertyName("screenshots")]
        public List<Screenshot>? Screenshots { get; set; }

        [JsonPropertyName("user_rate")]
        public object? UserRate { get; set; }

        public AnimeRole? AuthorInfo { get; set; }
    }

    public class AnimeRole
    {
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }

        [JsonPropertyName("person")]
        public Person Person { get; set; }
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

    public class ImageData
    {
        [JsonPropertyName("original")]
        public string Original { get; set; }

        [JsonPropertyName("preview")]
        public string Preview { get; set; }

        [JsonPropertyName("x96")]
        public string X96 { get; set; }

        [JsonPropertyName("x48")]
        public string X48 { get; set; }
    }

    public class RateScoreStat
    {
        [JsonPropertyName("name")]
        public int Name { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    public class RateStatusStat
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    public class Video
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("player_url")]
        public string PlayerUrl { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("hosting")]
        public string Hosting { get; set; }
    }

    public class Screenshot
    {
        [JsonPropertyName("original")]
        public string Original { get; set; }

        [JsonPropertyName("preview")]
        public string Preview { get; set; }
    }

}