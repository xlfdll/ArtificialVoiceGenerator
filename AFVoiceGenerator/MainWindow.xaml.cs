using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.Controls.Dialogs;

namespace AFVoiceGenerator
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

        private void InputTextTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainViewModel viewModel = this.DataContext as MainViewModel;

            if (viewModel != null)
            {
                viewModel.OutputText = PinYinSpell.ConvertToPinYin(viewModel.InputText);
            }
        }
    }
}