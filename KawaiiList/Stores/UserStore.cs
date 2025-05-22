using KawaiiList.Models;

namespace KawaiiList.Stores
{
    public class UserStore
    {
        public event Action CurrentUserAppChanged;

        private UserApp _currentUserApp;
        public UserApp CurrentUserApp
        {
            get => _currentUserApp;
            set
            {
                _currentUserApp = value;
                OnUserAppChanged();
            }
        }

        private void OnUserAppChanged()
        {
            CurrentUserAppChanged?.Invoke();
        }
    }
}
