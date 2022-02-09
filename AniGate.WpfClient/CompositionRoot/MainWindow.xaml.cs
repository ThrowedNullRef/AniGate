using System.Windows;
using System.Windows.Input;

namespace AniGate.WpfClient.CompositionRoot
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (e.ClickCount == 2)
            {
                SwitchWindowState();
            }
            else
            {
                if (Application.Current.MainWindow?.WindowState == WindowState.Maximized)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    var mousePos = e.GetPosition(this);
                    Application.Current.MainWindow.Left = mousePos.X - Application.Current.MainWindow.Width / 2.0;
                    Application.Current.MainWindow.Top = mousePos.Y - 20.0;
                }

                DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is null)
                return;

            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e) =>
            SwitchWindowState();

        private void Close_Click(object sender, RoutedEventArgs e) =>
            Application.Current?.Shutdown();

        private static void SwitchWindowState()
        {
            if (Application.Current.MainWindow is null)
                return;

            Application.Current.MainWindow.WindowState = Application.Current.MainWindow?.WindowState switch
            {
                WindowState.Maximized => WindowState.Normal,
                WindowState.Normal => WindowState.Maximized,
            };
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderThickness = WindowState == WindowState.Maximized ? new Thickness(8) : new Thickness(0);
        }
    }
}
