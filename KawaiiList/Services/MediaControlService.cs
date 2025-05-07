using KawaiiList.Components;
using KawaiiList.ViewModels;
using LibVLCSharp.Shared;
using System.Windows;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace KawaiiList.Services
{
    public class MediaControlService : IMediaControlService
    {
        private MediaPlayer? _mediaPlayer;
        private LibVLC? _libVLC;
        private bool _setMainWindwo = false;
        
        private readonly Func<FullscreenPlayerWindow> _windowFactory;
        private Window? _fullscreenWindow;
        
        public MediaControlService(Func<FullscreenPlayerWindow> windowFactory)
        {
            _windowFactory = windowFactory;
        }

        public void SetMediaPlayer(MediaPlayer player, LibVLC libVLC)
        {
            if(!_setMainWindwo)
            {
                _mediaPlayer = player;
                _libVLC = libVLC;
                _setMainWindwo = true;
            }
        }

        public void ToggleFullscreen(MediaPlayer player)
        {
            if (_fullscreenWindow == null)
            {
                _fullscreenWindow = _windowFactory();

                MediaPlayer copiedPlayer = new MediaPlayer(_libVLC)
                {
                    Media = _mediaPlayer.Media,
                    Volume = _mediaPlayer.Volume,
                };

                if (_fullscreenWindow.DataContext is WatchAnimeViewModel newVm)
                {
                    newVm.AnimeMediaPlayer = copiedPlayer;
                }

                if (_mediaPlayer.IsPlaying)
                {
                    copiedPlayer.Play();
                }

                copiedPlayer.Time = _mediaPlayer.Time;

                _fullscreenWindow.Show();
            }
            else
            {
                if (player.IsPlaying)
                {
                    _mediaPlayer.Play();
                }

                _mediaPlayer.Time = player.Time;

                _fullscreenWindow.Close();
                _fullscreenWindow = null;
            }
        }
    }
}