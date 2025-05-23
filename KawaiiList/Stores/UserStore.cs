using KawaiiList.Models;

namespace KawaiiList.Stores
{
    public class UserStore
    {
        public event Action CurrentUserChanged;

        public bool IsLoggedIn => CurrentUser != null;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnUserChanged();
            }
        }

        private void OnUserChanged()
        {
            CurrentUserChanged?.Invoke();
        }
    }
}
