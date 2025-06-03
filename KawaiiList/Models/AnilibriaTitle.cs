using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace KawaiiList.Models
{
    public class AnimeNames
    {
        public string? Ru { get; set; }
        public string? En { get; set; }
    }

    public class AnimePoster
    {
        private string? _url;

        public string? Url
        {
            get => _url;
            set => _url = value != null ? "https://www.anilibria.top" + value : null;
        }
    }

    public class AnimePosters
    {
        public AnimePoster? Original { get; set; }
    }

    public class SeasonAnime
    {
        public string? String { get; set; }
        public int? Year { get; set; }
    }

    public class EpisodesInfo
    {
        public double? First { get; set; }
        public double? Last { get; set; }
    }

    public class PlayerAnime
    {
        public EpisodesInfo? Episodes { get; set; }
        public string? Host { get; set; }
        public Dictionary<double, EpisodeInfo>? List { get; set; }
    }

    public class EpisodeInfo
    {
        public double? Episode { get; set; }
        public string? Name { get; set; }
        public Skips? Skips { get; set; }
        public HlsLinks? Hls { get; set; }
    }

    public class HlsLinks
    {
        public string? Fhd { get; set; }

        public string? Hd { get; set; }

        public string? Sd { get; set; }
    }

    public class Skips
    {
        public List<int>? Opening { get; set; }

        public List<int>? Ending { get; set; }
    }

    public class TypeInfo
    {
        public string? String { get; set; }
    }

    public class StatusInfo
    {
        public string? String { get; set; }
    }

    public class FranchiseInfo
    {
        public int Id { get; set; }
        public AnimeNames? Names { get; set; }
    }

    public class PaginationInfo
    {
        public int? Pages { get; set; }
        public int? CurrentPage { get; set; }
        public int? ItemsPerPage { get; set; }
        [JsonPropertyName("total_items")]
        public int? TotalItems { get; set; }
    }

    public partial class AnilibriaTitle : ObservableObject
    {
        public int Id { get; set; }
        public AnimeNames? Names { get; set; }
        public AnimePosters? Posters { get; set; }
        public List<string>? Genres { get; set; }
        public SeasonAnime? Season { get; set; }
        public PlayerAnime? Player { get; set; }

        [ObservableProperty]
        private string? _description;

        public TypeInfo? Type { get; set; }
        public StatusInfo? Status { get; set; }
        public List<FranchiseInfo>? Franchises { get; set; }

        public string GenresText => string.Join(" ", Genres ?? Enumerable.Empty<string>());

        public DateTime? UploadedAt { get; set; }

        public override string ToString()
        {
            return "";
        }
    }

    public class AnilibriaTitles
    {
        public List<AnilibriaTitle>? List { get; set; }
        public PaginationInfo? Pagination { get; set; }
    }

    public class ScheduleAnilibriaTitles
    {
        public int Day { get; set; }
        public List<AnilibriaTitle>? List { get; set; }
    }
}