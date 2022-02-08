using System.Threading.Tasks;
using System.Windows.Controls;

namespace AniCore.WpfClient.Animes
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
