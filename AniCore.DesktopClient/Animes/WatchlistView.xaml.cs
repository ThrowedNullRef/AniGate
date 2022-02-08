using System.Windows.Controls;

namespace AniCore.WpfClient.Animes
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
