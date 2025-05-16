using System.Windows;

namespace KawaiiList.Services
{
    public interface ICursorPositionService
    {
        event EventHandler<Point>? CursorPositionChanged;

        void Start();
        void Stop();
    }
}