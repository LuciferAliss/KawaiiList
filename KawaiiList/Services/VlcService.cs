using LibVLCSharp.Shared;

namespace KawaiiList.Services
{
    public class VlcService
    {
        public LibVLC LibVLC { get; }

        public VlcService()
        {
            Core.Initialize();
            LibVLC = new LibVLC();
        }

        public MediaPlayer CreatePlayer()
        {
            return new MediaPlayer(LibVLC);
        }

        public Media CreateMedia(string url)
        {
            return new Media(LibVLC, url, FromType.FromLocation);
        }
    }
}
