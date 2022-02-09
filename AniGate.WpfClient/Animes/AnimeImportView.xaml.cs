using System.Windows.Controls;

namespace AniGate.WpfClient.Animes
{
    public partial class AnimeImportView : UserControl
    {
        public AnimeImportView(AnimeImportViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
