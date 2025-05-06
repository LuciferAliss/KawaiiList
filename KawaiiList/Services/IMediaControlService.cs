using System.Windows;

namespace KawaiiList.Services
{
    public interface IMediaControlService
    {
        event Action<UIElement?, object?>? OnRequestFullscreen;
        event Action? OnRequestExitFullscreen;

        void EnterFullscreen(UIElement? content, object? viewModel);
        void ExitFullscreen();

        void Play();
        void Pause();
        void TogglePlay();
        void Seek(long position);
        void SetVolume(int volume);
        void ToggleMute();

        bool IsPlaying { get; }
        long Duration { get; }
        long CurrentTime { get; }
        int Volume { get; set; }
    }
}