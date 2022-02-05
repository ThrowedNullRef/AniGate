using AniCore.Core.AnimeProviders;
using AniCore.Core.DataAccess;
using AniCore.RavenDataAccess;
using AniCore.RavenDataAccess.Sessions;
using LightInject;
using Raven.Client.Documents;

namespace AniCore.DesktopClient.CompositionRoot
{
    public static class RegistryExtensions
    {
        public static IServiceRegistry RegisterStartup(this IServiceRegistry registry) =>
            registry.RegisterTransient<MainWindow>()
                    .RegisterTransient<MainWindowViewModel>();

        public static IServiceRegistry RegisterDataAccess(this IServiceRegistry registry) =>
            registry.RegisterInstance(RavenDb.InitializeStore(App.DataDirectory))
                    .Register(f => f.GetInstance<IDocumentStore>().OpenAsyncSession());

        public static IServiceRegistry RegisterAnimeProvider(this IServiceRegistry registry) =>
            registry.RegisterTransient<IAnimeProvider, AnimeToastAnimeProvider>();

        public static IServiceRegistry RegisterWatchlist(this IServiceRegistry registry) =>
            registry.RegisterTransient<IWatchlistSession, RavenWatchlistSession>();
    }
}
