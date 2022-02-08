using AniCore.Core.AnimeProviders;
using AniCore.Core.AnimeSynchronization;
using AniCore.Core.DataAccess;
using AniCore.RavenDataAccess;
using AniCore.RavenDataAccess.Sessions;
using AniCore.WpfClient.Animes;
using LightInject;
using Raven.Client.Documents;

namespace AniCore.WpfClient.CompositionRoot
{
    public static class RegistryExtensions
    {
        public static IServiceRegistry RegisterCore(this IServiceRegistry registry) =>
            registry.Register<MainWindow>(new PerContainerLifetime())
                    .Register(f => new MainWindowViewModel(f, f.GetInstance<IAnimeSynchronizer>()), new PerContainerLifetime())
                    .Register<INavigator>(f => f.GetInstance<MainWindowViewModel>());

        public static IServiceRegistry RegisterDataAccess(this IServiceRegistry registry) =>
            registry.RegisterInstance(RavenDb.InitializeStore(App.DataDirectory))
                    .Register(f =>
                     {
                         var session = f.GetInstance<IDocumentStore>().OpenAsyncSession();
                         session.Advanced.WaitForIndexesAfterSaveChanges();
                         return session;
                     });

        public static IServiceRegistry RegisterAnimeProvider(this IServiceRegistry registry) =>
            registry.Register<IAnimeProvider, AnimeToastAnimeProvider>();

        public static IServiceRegistry RegisterWatchlist(this IServiceRegistry registry) =>
            registry.Register<WatchlistView>()
                    .Register<WatchlistViewModel>()
                    .Register<IWatchlistSession, RavenWatchlistSession>();

        public static IServiceRegistry RegisterAnimePlayer(this IServiceRegistry registry) =>
            registry.Register<AnimePlayerView>()
                    .Register<AnimePlayerViewModel>()
                    .Register<IAnimePlayerSession, RavenAnimePlayerSession>();

        public static IServiceRegistry RegisterAnimeSynchronization(this IServiceRegistry registry) =>
            registry.Register<IAnimeSynchronizationSession, RavenAnimeSynchronizationSession>()
                    .RegisterSingleton<IAnimeSynchronizer, AnimeSynchronizer>();

        public static IServiceRegistry RegisterAnimes(this IServiceRegistry registry) =>
            registry.Register<AnimesView>()
                    .Register<AnimesViewModel>()
                    .Register<IAnimesSession, RavenAnimesSession>();

        public static IServiceRegistry RegisterAnimeImport(this IServiceRegistry registry) =>
            registry.Register<AnimeImportView>()
                    .Register<AnimeImportViewModel>()
                    .Register<IAnimeImportSession, RavenAnimeImportSession>();
    }
}
