using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;

namespace AniGate.WpfClient.Common
{
    public partial class WebPlayerView : UserControl
    {
        private Window? _fullScreenWindow;
        private Panel? _prevParent;

        public WebPlayerView()
        {
            InitializeComponent();
            WebView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        ~WebPlayerView()
        {
            WebView.CoreWebView2InitializationCompleted -= WebView_CoreWebView2InitializationCompleted;
            if (WebView.CoreWebView2 is null)
                return;

            WebView.CoreWebView2.NewWindowRequested -= CoreWebView2_NewWindowRequested;
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            WebView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            WebView.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            if (WebView.CoreWebView2.ContainsFullScreenElement && _fullScreenWindow is not null ||
                !WebView.CoreWebView2.ContainsFullScreenElement && _fullScreenWindow is null)
            {
                return;
            }

            ToggleFullScreen();
        }

        private void ToggleFullScreen()
        {
            if (_fullScreenWindow is null)
            {
                EnterFullScreen();
            }
            else
            {
                ExitFullScreen();
            }
        }

        private void EnterFullScreen()
        {
            if (VisualTreeHelper.GetParent(this) is Panel parent)
            {
                _prevParent = parent;
                parent.Children.Remove(this);
            }

            _fullScreenWindow = new Window
            {
                Content = this,
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Maximized,
            };

            _fullScreenWindow.KeyDown += (_, __) => ToggleFullScreen();
            _fullScreenWindow.Show();
        }

        private void FullScreenWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ToggleFullScreen();
        }

        private void ExitFullScreen()
        {
            if (_fullScreenWindow is null)
                return;

            if (_prevParent is not null && _fullScreenWindow.Content is WebPlayerView webPlayer)
            {
                _fullScreenWindow.Content = null;
                _prevParent.Children.Add(webPlayer);
                _prevParent = null;
            }

            _fullScreenWindow.Close();
            _fullScreenWindow = null;
        }
    }
}
