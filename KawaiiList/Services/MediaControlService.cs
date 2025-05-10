namespace KawaiiList.Services
{
    public class MediaControlService : IMediaControlService
    {
        public event Action FullscreenModeChanged;

        private bool _isFullscreen = true;
        public bool IsFullscreen
        {
            get => _isFullscreen;
            set
            {
                _isFullscreen = value;
                OnFullscreenModeChanged();
            }
        }

        private void OnFullscreenModeChanged()
        {
            FullscreenModeChanged?.Invoke();
        }
    }
}