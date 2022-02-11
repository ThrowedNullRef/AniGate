using System;
using AniGate.Core;

namespace AniGate.WpfClient.CompositionRoot;

public interface INavigator
{
    void NavigateToWatchlist();

    void NavigateToAnimes();

    void NavigateToAnimeImport();

    void NavigateToPlayer(Anime anime);

    void NavigateBack();


    event Action<object?> OnNavigated;
}
