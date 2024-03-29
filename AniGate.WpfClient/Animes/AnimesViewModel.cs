﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using AniGate.Core;
using AniGate.Core.AnimeSynchronization;
using AniGate.Core.DataAccess;
using AniGate.WpfClient.CompositionRoot;
using AniGate.WpfClient.FrameworkExtensions;

namespace AniGate.WpfClient.Animes;

public sealed class AnimesViewModel : BaseNotifyPropertyChanged
{
    private readonly Func<IAnimesSession> _createSession;
    private readonly IAnimeSynchronizer _animeSynchronizer;
    private bool _isReloading;

    public AnimesViewModel(Func<IAnimesSession> createSession, INavigator navigator, IAnimeSynchronizer animeSynchronizer)
    {
        _createSession = createSession;
        _animeSynchronizer = animeSynchronizer;
        AnimeListViewModel = new AnimeListViewModel(new List<Anime>(), new AnimeCardActions(navigator.NavigateToPlayer)
        {
            OnAddToWatchlist = OnAddToWatchlist,
            OnRemoveFromWatchlist = OnRemoveFromWatchlist,
            OnDelete = OnDelete
        });

        Initialize();

        _animeSynchronizer.OnSynchronized += AnimeSynchronizer_OnSynchronized;
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

    private async void OnAddToWatchlist(Anime anime)
    {
        using var session = _createSession();
        var trackedAnime = await session.GetAnimeAsync(anime.Id);
        trackedAnime.IsWatching = true;
        await session.SaveChangesAsync();
        await ReloadAsync();
    }

    private async void OnDelete(Anime anime)
    {
        using var session = _createSession();
        var trackedAnime = await session.GetAnimeAsync(anime.Id);
        session.DeleteAnime(trackedAnime);
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
            var animes = await session.GetAnimesAsync();
            AnimeListViewModel.SetAnimes(animes);
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