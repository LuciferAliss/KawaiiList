using System.Windows;

namespace KawaiiList.Services
{
    public class ScreenService : IScreenService
    {
        public double GetScreenWidth() => SystemParameters.PrimaryScreenWidth;
        public double GetScreenHeight() => SystemParameters.PrimaryScreenHeight;
    }
}