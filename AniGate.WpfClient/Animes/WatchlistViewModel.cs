using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AniGate.Core;
using AniGate.Core.AnimeSynchronization;
using AniGate.Core.DataAccess;
using AniGate.WpfClient.CompositionRoot;
using AniGate.WpfClient.FrameworkExtensions;

namespace AniGate.WpfClient.Animes;

public sealed class WatchlistViewModel : BaseNotifyPropertyChanged
{
    private readonly Func<IWatchlistSession> _createSession;
    private readonly IAnimeSynchronizer _animeSynchronizer;
    private bool _isReloading;

    public WatchlistViewModel(Func<IWatchlistSession> createSession, INavigator navigator, IAnimeSynchronizer animeSynchronizer)
    {
        _createSession = createSession;
        _animeSynchronizer = animeSynchronizer;
        AnimeListViewModel = new AnimeListViewModel(new List<Anime>(), new AnimeCardActions()
        {
            OnPlay = navigator.NavigateToPlayer,
            OnRemoveFromWatchlist = OnRemoveFromWatchlist
        });

        animeSynchronizer.OnSynchronized += AnimeSynchronizer_OnSynchronized;

        Initialize();
    }
   
    ~WatchlistViewModel()
    {
        _animeSynchronizer.OnSynchronized -= AnimeSynchronizer_OnSynchronized;
    }

    public AnimeListViewModel AnimeListViewModel { get; }

    private async void Initialize()
    {
        await ReloadAsync();
    }

    private async void OnRemoveFromWatchlist(Anime anime)
    {
        using var session = _createSession();
        var trackedAnime = await session.GetAnimeAsync(anime.Id);
        trackedAnime.IsWatching = false;
        await session.SaveChangesAsync();
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        if (_isReloading)
            return;

        try
        {
            _isReloading = true;
            using var session = _createSession();
            var animes = await session.GetWatchlistAnimesAsync();
            AnimeListViewModel.SetAnimes(animes.OrderByDescending(a => a.HasUnwatchedEpisode()).
                                                ThenBy(a => a.OriginalName)
                                               .ToList());
        }
        finally
        {
            _isReloading = false;
        }
    }

    private async void AnimeSynchronizer_OnSynchronized(List<Anime> obj)
    {
        await Application.Current.Dispatcher.BeginInvoke(async () => await ReloadAsync());
    }
}