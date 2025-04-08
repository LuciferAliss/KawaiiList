using KawaiiList.ViewModels.MainVm;
using System.Windows;

namespace KawaiiList;

public partial class MainPage  : Window
{
    public MainPage(IMainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}