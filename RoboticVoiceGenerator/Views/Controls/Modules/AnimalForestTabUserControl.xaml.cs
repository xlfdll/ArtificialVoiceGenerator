using System.Windows.Controls;

namespace RoboticVoiceGenerator
{
    /// <summary>
    /// AnimalForestTabUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class AnimalForestTabUserControl : UserControl
    {
        public AnimalForestTabUserControl()
        {
            InitializeComponent();
        }

        private void InputTextTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AFViewModel viewModel = this.DataContext as AFViewModel;

            if (viewModel != null)
            {
                viewModel.OutputText = PinYinSpell.ConvertToPinYin(viewModel.InputText);
            }
        }
    }
}