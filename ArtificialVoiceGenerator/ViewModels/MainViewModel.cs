using System.Resources;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll;

namespace ArtificialVoiceGenerator
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.DialogCoordinator = dialogCoordinator;
            this.ResourceManager = Properties.Resources.ResourceManager;
        }

        public IDialogCoordinator DialogCoordinator { get; }
        public ResourceManager ResourceManager { get; }

        public AFViewModel AFViewModel
            => new AFViewModel(this);
    }
}