using KawaiiList.Stores;

namespace KawaiiList.Services
{
    public class CloseModalNavigationService : ICloseModalNavigationService
    {
        private readonly ModalNavigationStore _modalNavigationStore;

        public CloseModalNavigationService(ModalNavigationStore modalNavigationStore)
        {
            _modalNavigationStore = modalNavigationStore;
        }

        public void Navigate()
        {
            _modalNavigationStore.Close();
        }
    }
}
