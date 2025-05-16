using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace KawaiiList.Services
{
    public class CursorPositionService : ICursorPositionService
    {
        private readonly DispatcherTimer _timer;
        private Point _lastPosition;

        public event EventHandler<Point>? CursorPositionChanged;

        public CursorPositionService(int checkIntervalMs = 150)
        {
            _lastPosition = GetCursorPosition();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(checkIntervalMs)
            };
            _timer.Tick += Timer_Tick;
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var currentPos = GetCursorPosition();
            if (!currentPos.Equals(_lastPosition))
            {
                _lastPosition = currentPos;
                CursorPositionChanged?.Invoke(this, currentPos);
            }
        }

        private Point GetCursorPosition()
        {
            var pos = System.Windows.Forms.Control.MousePosition;
            return new Point(pos.X, pos.Y);
        }
    }
}
