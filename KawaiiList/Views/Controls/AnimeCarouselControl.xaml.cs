using KawaiiList.ViewModels.AnimeCarouselVM;
using System.Windows.Controls;

namespace KawaiiList.Views.Controls
{
    public partial class AnimeCarouselControl : UserControl
    {
        public AnimeCarouselControl(IAnimeCarouselViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
