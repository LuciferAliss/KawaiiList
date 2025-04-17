using KawaiiList.ViewModels.MainVm;
using System.Windows;
using System.Windows.Input;

namespace KawaiiList;

public partial class MainPage  : Window
{
    public MainPage(IMainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void WindowMouseDoun(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}