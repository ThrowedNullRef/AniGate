using System.Windows;

namespace AniCore.DesktopClient.CompositionRoot
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.ReloadAnimesAsync();
            InitializeComponent();
        }
    }
}
