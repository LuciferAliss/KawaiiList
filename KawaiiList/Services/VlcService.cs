using KawaiiList.Models;
using LibVLCSharp.Shared;
using System.Diagnostics;

namespace KawaiiList.Services
{
    public class VlcService
    {
        public event Action FullScreenModeChanged;
        private readonly string host;
        public LibVLC LibVLC { get; }
        public MediaPlayer AnimeMediaPlayer;
        private bool _isFullScreen = false;

        public bool IsFullScreen
        {
            get => _isFullScreen;
            set
            {
                _isFullScreen = value;
                OnFullScreenModeChanged();
            }
        }

        public VlcService()
        {
            Core.Initialize();
            LibVLC = new LibVLC();
            AnimeMediaPlayer = new MediaPlayer(LibVLC);
            host = "https://cache.libria.fun";
        }

        public Media CreateMedia(string url)
        {
            return new Media(LibVLC, host + url, FromType.FromLocation);
        }

        public void FullScreenMode()
        {
            IsFullScreen = !IsFullScreen;
            AnimeMediaPlayer.Fullscreen = IsFullScreen;

            Debug.WriteLine(AnimeMediaPlayer.Fullscreen);
        }

        private void OnFullScreenModeChanged()
        {
            FullScreenModeChanged?.Invoke();
        }
    }
}
