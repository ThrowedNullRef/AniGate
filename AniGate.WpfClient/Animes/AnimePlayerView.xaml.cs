using System.Windows.Controls;

namespace AniGate.WpfClient.Animes
{
    /// <summary>
    /// Interaction logic for AnimePlayerView.xaml
    /// </summary>
    public partial class AnimePlayerView : UserControl
    {
        private readonly AnimePlayerViewModel _viewModel;

        public AnimePlayerView(AnimePlayerViewModel viewModel)
        {
            DataContext = _viewModel = viewModel;
            InitializeComponent();
        }
    }
}
