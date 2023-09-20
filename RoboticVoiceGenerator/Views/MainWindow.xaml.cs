using System.Windows;

using MahApps.Metro.Controls.Dialogs;

namespace RoboticVoiceGenerator
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel(DialogCoordinator.Instance);
        }
    }
}