using KawaiiList.ViewModels.HomeVM;
using System;
using System.Windows.Controls;

namespace KawaiiList.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomeControl : UserControl
    {
        public HomeControl(IHomeViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
