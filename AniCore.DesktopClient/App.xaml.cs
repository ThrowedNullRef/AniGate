using System.Diagnostics;
using System.IO;
using LightInject;
using System.Windows;
using AniCore.DesktopClient.CompositionRoot;

namespace AniCore.DesktopClient
{
    public partial class App : Application
    {
        private readonly ServiceContainer _container = new ();
        public static readonly string AppFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;
        public static readonly string DataDirectory = Path.Combine(AppFolder, "Data");

        public App()
        {
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            _container.RegisterStartup()
                      .RegisterDataAccess()
                      .RegisterAnimeProvider()
                      .RegisterWatchlist();
        }

        private void Application_Startup(object sender, StartupEventArgs e) =>
            _container.GetInstance<MainWindow>().Show();
    }
}
