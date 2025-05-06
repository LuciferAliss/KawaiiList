using KawaiiList.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KawaiiList.Views
{
    public partial class WatchAnimeView : UserControl
    {
        public WatchAnimeView(IMediaControlService mediaService)
        {
            InitializeComponent();

            mediaService.OnRequestFullscreen += (content, viewModel) =>
            {
                if (content == null)
                {
                    content = Player;
                }

                mediaService.EnterFullscreen(content, viewModel);
            };
        }
    }
}
