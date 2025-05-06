using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace KawaiiList.Services
{
    public class MediaControlService : IMediaControlService
    {
        public event Action<UIElement?, object?>? OnRequestFullscreen;
        public event Action? OnRequestExitFullscreen;

        private Window? _fullscreenWindow;
        private MediaPlayer? _mediaPlayer;

        public void SetMediaPlayer(MediaPlayer player)
        {
            _mediaPlayer = player;
        }

        public bool IsPlaying => _mediaPlayer?.IsPlaying ?? false;

        public long Duration => _mediaPlayer?.Media?.Duration ?? 0;

        public long CurrentTime => _mediaPlayer?.Time ?? 0;

        public int Volume
        {
            get => _mediaPlayer?.Volume ?? 0;
            set
            {
                if (_mediaPlayer != null)
                {
                    _mediaPlayer.Volume = value;
                }
            }
        }

        public void Play()
        {
            _mediaPlayer?.Play();
        }
        public void Pause()
        {
            _mediaPlayer?.Pause();
        }
        public void TogglePlay()
        {
            if (_mediaPlayer == null) return;
            if (_mediaPlayer.IsPlaying) Pause(); else Play();
        }

        public void Seek(long position)
        {
            if (_mediaPlayer != null && _mediaPlayer.Length > 0)
            {
                _mediaPlayer.Time = Math.Clamp(position, 0, _mediaPlayer.Length);
            }
        }

        public void SetVolume(int volume)
        { 
            Volume = volume; 
        }

        public void ToggleMute()
        {
            if (_mediaPlayer != null)
                _mediaPlayer.Mute = !_mediaPlayer.Mute;
        }

        public void EnterFullscreen(UIElement? content, object? viewModel)
        {
            OnRequestFullscreen?.Invoke(content, viewModel);
        }

        public void ExitFullscreen()
        {
            _fullscreenWindow?.Close();
            _fullscreenWindow = null;
            OnRequestExitFullscreen?.Invoke();
        }

        public void ShowFullscreenWindow(UIElement content, object? viewModel)
        {
            DetachFromParent(content);

            _fullscreenWindow = new Window
            {
                WindowState = WindowState.Maximized,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                AllowsTransparency = false,
                Background = Brushes.Black,
                Content = content,
                DataContext = viewModel,
                Topmost = true
            };

            _fullscreenWindow.Show();

            _fullscreenWindow.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                    ExitFullscreen();
            };

            _fullscreenWindow.Closed += (s, e) => _fullscreenWindow = null;
        }

        private static void DetachFromParent(UIElement element)
        {
            var parent = LogicalTreeHelper.GetParent(element);

            switch (parent)
            {
                case Panel panel:
                    panel.Children.Remove(element);
                    break;
                case ContentControl contentControl when contentControl.Content == element:
                    contentControl.Content = null;
                    break;
                case Decorator decorator when decorator.Child == element:
                    decorator.Child = null;
                    break;
                case ItemsControl itemsControl:
                    itemsControl.Items.Remove(element);
                    break;
            }
        }
    }
}
