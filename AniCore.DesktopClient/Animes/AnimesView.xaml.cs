using System.Windows.Controls;

namespace AniCore.WpfClient.Animes
{
    /// <summary>
    /// Interaction logic for AnimesView.xaml
    /// </summary>
    public partial class AnimesView : UserControl
    {
        public AnimesView(AnimesViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
