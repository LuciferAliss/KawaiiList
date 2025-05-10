using KawaiiList.ViewModels;
using LibVLCSharp.Shared;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace KawaiiList.Services
{
    public interface IMediaControlService
    {
        public event Action FullscreenModeChanged;

        public bool IsFullscreen { get; set; }
    }
}