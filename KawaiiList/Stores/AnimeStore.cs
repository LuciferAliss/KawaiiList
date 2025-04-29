using KawaiiList.Models;

namespace KawaiiList.Stores
{
    public class AnimeStore
    {
        public event Action CurrentAnimeChanged;

        private AnimeTitle _currentAnime;
        public AnimeTitle CurrentAnime
        {
            get => _currentAnime;
            set
            {
                _currentAnime = value;
                OnCurrentAnimeChanged();
            }
        }

        private ShikimoriTitle _currentAnimeInfo;
        public ShikimoriTitle CurrentAnimeInfo
        {
            get => _currentAnimeInfo;
            set
            {
                _currentAnimeInfo = value;
                OnCurrentAnimeChanged();
            }
        }

        private void OnCurrentAnimeChanged()
        {
            CurrentAnimeChanged?.Invoke();
        }
    }
}