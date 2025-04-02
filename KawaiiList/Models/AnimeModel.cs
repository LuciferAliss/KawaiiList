namespace KawaiiList.Models
{
    public class AnimeItem
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
        public AnimePoster? Medium { get; set; }
        public AnimePoster? Small { get; set; }
    }

    public class AnimeType
    {
        public string? String { get; set; }
        public int? Year { get; set; }
    }

    public class Anime
    {
        public int Id { get; set; }
        public AnimeItem? Names { get; set; }
        public AnimePosters? Posters { get; set; }
        public List<string>? Genres { get; set; }
        public AnimeType? Season { get; set; }

        public string GenresText => string.Join(" ", Genres);
    }

    public class AnimeResponse
    {
        public List<Anime>? List { get; set; }
    }
}
