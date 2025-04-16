using KawaiiList.ViewModels.SearchVM;
using System.Windows.Controls;

namespace KawaiiList.Views.Controls
{
    public partial class SearchControl : UserControl
    {
        public SearchControl(ISearchViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
