using LibVLCSharp.Shared;

namespace KawaiiList.Services
{
    public class VlcService
    {
        private readonly string host;
        public LibVLC LibVLC { get; }

        public VlcService()
        {
            Core.Initialize();
            LibVLC = new LibVLC();
            host = "https://cache.libria.fun";
        }

        public MediaPlayer CreatePlayer()
        {
            return new MediaPlayer(LibVLC);
        }

        public Media CreateMedia(string url)
        {
            return new Media(LibVLC, host + url, FromType.FromLocation);
        }
    }
}
