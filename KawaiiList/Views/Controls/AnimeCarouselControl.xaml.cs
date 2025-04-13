using KawaiiList.ViewModels.AnimeCarouselVM;
using System.Diagnostics;
using System.Windows.Controls;

namespace KawaiiList.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для AnimeCarousel.xaml
    /// </summary>
    public partial class AnimeCarouselControl : UserControl
    {
        public AnimeCarouselControl(IAnimeCarouselViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void AnimeCarousel_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("11");
        }
    }
}
