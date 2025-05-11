using LibVLCSharp.Shared;
using System.Windows;
using System.Windows.Input;

namespace KawaiiList;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Core.Initialize();

        Height = 720;
        Width = 1280;
    }

    private void WindowMouseDoun(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}