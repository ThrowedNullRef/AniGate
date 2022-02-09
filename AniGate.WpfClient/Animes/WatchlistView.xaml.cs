using System.Windows.Controls;

namespace AniGate.WpfClient.Animes
{
    /// <summary>
    /// Interaction logic for WatchlistView.xaml
    /// </summary>
    public partial class WatchlistView : UserControl
    {
        public WatchlistView(WatchlistViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
