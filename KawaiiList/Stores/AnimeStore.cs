using KawaiiList.Models.Anilibria;

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

        private void OnCurrentAnimeChanged()
        {
            CurrentAnimeChanged?.Invoke();
        }
    }
}