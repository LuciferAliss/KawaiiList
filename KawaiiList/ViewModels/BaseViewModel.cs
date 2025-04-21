using CommunityToolkit.Mvvm.ComponentModel;

namespace KawaiiList.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        public virtual void Dispose() { }
    }
}
