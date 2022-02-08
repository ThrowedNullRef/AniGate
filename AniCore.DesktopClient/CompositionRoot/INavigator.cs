using AniCore.Core;

namespace AniCore.WpfClient.CompositionRoot;

public interface INavigator
{
    void NavigateToWatchlist();

    void NavigateToAnimes();

    void NavigateToAnimeImport();

    void NavigateToPlayer(Anime anime);
}
