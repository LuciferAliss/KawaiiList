namespace KawaiiList.Models.Anilibria
{
    public class AnimeNames
    {
        public string? Ru { get; set; }
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
        public int First { get; set; }
        public int Last { get; set; }
    }

    public class PlayerAnime
    {
        public EpisodesInfo? Episodes { get; set; }
    }

    public class AnimeTitle
    {
        public int Id { get; set; }
        public AnimeNames? Names { get; set; }
        public AnimePosters? Posters { get; set; }
        public List<string>? Genres { get; set; }
        public SeasonAnime? Season { get; set; }
        public PlayerAnime? Player { get; set; }

        public string GenresText => string.Join(" ", Genres ?? Enumerable.Empty<string>());

        public override string ToString()
        {
            return "";
        }
    }
}