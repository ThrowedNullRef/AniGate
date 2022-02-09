using System.Windows.Controls;

namespace AniGate.WpfClient.Animes
{
    /// <summary>
    /// Interaction logic for AnimePlayerView.xaml
    /// </summary>
    public partial class AnimePlayerView : UserControl
    {
        public AnimePlayerView(AnimePlayerViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
