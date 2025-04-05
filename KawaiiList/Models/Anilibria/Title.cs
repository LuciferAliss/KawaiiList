using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public AnimePoster? Medium { get; set; }
        public AnimePoster? Small { get; set; }
    }

    public class AnimeType
    {
        public string? String { get; set; }
        public int? Year { get; set; }
    }

    public class AnimeTitle
    {
        public int Id { get; set; }
        public AnimeNames? Names { get; set; }
        public AnimePosters? Posters { get; set; }
        public List<string>? Genres { get; set; }
        public AnimeType? Season { get; set; }

        public string GenresText => string.Join(" ", Genres ?? Enumerable.Empty<string>());
    }
}
