using CommunityToolkit.Mvvm.ComponentModel;

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
            set => _url = value != null ? "https://www.anilibria.tv" + value : null;
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
        public int? First { get; set; }
        public int? Last { get; set; }
    }

    public class PlayerAnime
    {
        public EpisodesInfo? Episodes { get; set; }
        public string? Host { get; set; }
        public Dictionary<string, EpisodeInfo>? List { get; set; }
    }

    public class EpisodeInfo
    {
        public int? Episode { get; set; }
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

    public partial class AnimeTitle : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private AnimeNames? _names;

        [ObservableProperty]
        private AnimePosters? _posters;

        [ObservableProperty]
        private List<string>? _genres;

        [ObservableProperty]
        private SeasonAnime? _season;

        [ObservableProperty]
        private PlayerAnime? _player;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        private TypeInfo? _type;

        [ObservableProperty]
        private StatusInfo? _status;

        [ObservableProperty]
        private List<FranchiseInfo>? _franchises;

        public string GenresText => string.Join(" ", Genres ?? Enumerable.Empty<string>());

        public override string ToString()
        {
            return "";
        }
    }
}