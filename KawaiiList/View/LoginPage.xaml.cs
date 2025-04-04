using System.Windows;
using System.Windows.Input;

namespace KawaiiList.View
{
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void WindowMouseDoun(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeWindowClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindowClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
