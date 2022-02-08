using Microsoft.Web.WebView2.Core;
using System;
using System.Windows.Controls;
using AniCore.WpfClient.CompositionRoot;

namespace AniCore.WpfClient.Animes
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
            WebView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        ~AnimePlayerView()
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

        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            _viewModel.ToggleFullScreen();
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
