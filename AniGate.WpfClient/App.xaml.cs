using System.Diagnostics;
using System.IO;
using System.Windows;
using AniGate.Core;
using AniGate.WpfClient.CompositionRoot;
using LightInject;

namespace AniGate.WpfClient
{
    public partial class App : Application
    {
        private readonly ServiceContainer _container = new ();
        public static readonly string AppFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;
        public static readonly string DataDirectory = Path.Combine(AppFolder, "Data");

        public App()
        {
            ConfigureServices();
            RunHostedServices();
        }

        private void ConfigureServices()
        {
            _container.RegisterInstance<IServiceContainer>(_container)
                      .RegisterCore()
                      .RegisterDataAccess()
                      .RegisterAnimeProvider()
                      .RegisterAnimes()
                      .RegisterWatchlist()
                      .RegisterAnimePlayer()
                      .RegisterAnimeImport()
                      .RegisterAnimeSynchronization();
        }

        private void Application_Startup(object sender, StartupEventArgs e) =>
            _container.GetInstance<MainWindow>()
                      .Show();

        private void RunHostedServices()
        {
            var services = _container.GetAllInstances<IHostedService>();
            foreach (var service in services)
            {
                service.Start();
            }
        }
    }
}
