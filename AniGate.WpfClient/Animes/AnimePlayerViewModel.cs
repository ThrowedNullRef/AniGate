using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AniGate.Core;
using AniGate.Core.AnimeSynchronization;
using AniGate.Core.DataAccess;
using AniGate.WpfClient.Common;
using AniGate.WpfClient.CompositionRoot;
using AniGate.WpfClient.FrameworkExtensions;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;

namespace AniGate.WpfClient.Animes;

public sealed class AnimePlayerViewModel : BaseNotifyPropertyChanged
{
    private readonly Func<IAnimePlayerSession> _createSession;
    private readonly IAnimeSynchronizer _animeSynchronizer;
    private readonly INavigator _navigator;
    private List<FirstLevelNavigationItem> _episodeItems = new ();
    private Episode? _currentEpisode;
    private WebPlayerViewModel? _webPlayerViewModel;
    private Anime? _anime;
    private FirstLevelNavigationItem? _currentEpisodeItem;

    public AnimePlayerViewModel(Func<IAnimePlayerSession> createSession, IAnimeSynchronizer animeSynchronizer, INavigator navigator)
    {
        _createSession = createSession;
        _animeSynchronizer = animeSynchronizer;
        _navigator = navigator;
        NavigateBackCommand = new DelegateCommand(navigator.NavigateBack);
        ToggleWatchedCommand = new DelegateCommand(ToggleCurrentEpisodeWatchedStatusAsync, () => CurrentEpisode is not null);
        PreviousEpisodeCommand = new DelegateCommand(PreviousEpisode, () => CurrentEpisode?.Position != 0);
        NextEpisodeCommand = new DelegateCommand(NextEpisode, () => CurrentEpisode?.Position != Anime?.Episodes.Max(e => e.Position));
        OpenInBrowserCommand = new DelegateCommand(() => CurrentEpisode?.OpenInBrowser(), () => CurrentEpisode is not null);
        NavItemSelectedCommand = new ParameterizedCommand<NavigationItem>(OnNavItemSelected);
        animeSynchronizer.OnSynchronized += AnimeSynchronizer_OnSynchronized;
        navigator.OnNavigated += Navigator_OnNavigated;
    }

    public Anime? Anime
    {
        get => _anime;
        private set => SetIfDifferent(ref _anime, value);
    }

    public Episode? CurrentEpisode
    {
        get => _currentEpisode;
        set
        {
            SetIfDifferent(ref _currentEpisode, value);
            if (value is not null)
                WebPlayerViewModel = new WebPlayerViewModel(new Uri(value.VoeLink));

            NextEpisodeCommand.RaiseCanExecuteChanged();
            PreviousEpisodeCommand.RaiseCanExecuteChanged();
            ToggleWatchedCommand.RaiseCanExecuteChanged();
            OpenInBrowserCommand.RaiseCanExecuteChanged();
            CurrentEpisodeItem = EpisodeItems.FirstOrDefault(ei => ei.Label == value?.ToString());
        }
    }

    public List<FirstLevelNavigationItem> EpisodeItems
    {
        get => _episodeItems;
        private set => SetIfDifferent(ref _episodeItems, value);
    }

    public FirstLevelNavigationItem? CurrentEpisodeItem
    {
        get => _currentEpisodeItem;
        set => SetIfDifferent(ref _currentEpisodeItem, value);
    }

    public WebPlayerViewModel? WebPlayerViewModel
    {
        get => _webPlayerViewModel;
        set => SetIfDifferent(ref _webPlayerViewModel, value);
    }

    public ParameterizedCommand<NavigationItem> NavItemSelectedCommand { get; }

    public DelegateCommand NavigateBackCommand { get; }

    public DelegateCommand ToggleWatchedCommand { get; }

    public DelegateCommand PreviousEpisodeCommand { get; }

    public DelegateCommand NextEpisodeCommand { get; }

    public DelegateCommand OpenInBrowserCommand { get; }

    public async Task LoadAnimeAsync(string animeId, bool tryKeepSelectedPosition = false)
    {
        int? wantedSelectionPos = tryKeepSelectedPosition && CurrentEpisode is not null ? CurrentEpisode.Position : null;

        using var session = _createSession();
        CurrentEpisode = null;
        Anime = await session.GetAnimeAsync(animeId);

        var foundSelection = false;
        var newEpisodeItems = new List<FirstLevelNavigationItem>(Anime.Episodes.Count);
        foreach (var episode in Anime.Episodes.OrderBy(e => e.Position))
        {
            var item = new FirstLevelNavigationItem
            {
                Label = episode.ToString(),
                IsSelected = !foundSelection && !wantedSelectionPos.HasValue && !episode.IsWatched || 
                             !foundSelection && wantedSelectionPos.HasValue && episode.Position == wantedSelectionPos,
                Icon = episode.IsWatched ? PackIconKind.EyeCheck : PackIconKind.Eye,
            };

            if (item.IsSelected)
                foundSelection = true;

            newEpisodeItems.Add(item);
        }

        if (!foundSelection && newEpisodeItems.Any())
            newEpisodeItems.First().IsSelected = true;

        EpisodeItems = newEpisodeItems;
    }

    private void OnNavItemSelected(NavigationItem? item)
    {
        if (item is null)
            return;

        CurrentEpisode = Anime?.Episodes.First(e => e.ToString() == item.Label);
    }

    private async void ToggleCurrentEpisodeWatchedStatusAsync()
    {
        if (CurrentEpisode is null)
            return;

        await UpdateCurrentEpisodeWatchedStatus(!CurrentEpisode.IsWatched);
    }

    private async void NextEpisode()
    {
        if (CurrentEpisode is null || Anime is null)
            return;

        await UpdateCurrentEpisodeWatchedStatus(true);

        var nextEpisode = Anime.Episodes.FirstOrDefault(e => e.Position == CurrentEpisode.Position + 1);
        if (nextEpisode is null)
            return;

        CurrentEpisode = nextEpisode;
    }

    private void PreviousEpisode()
    {
        if (CurrentEpisode is null || Anime is null)
            return;

        var previousEpisode = Anime.Episodes.FirstOrDefault(e => e.Position == CurrentEpisode.Position - 1);
        if (previousEpisode is null)
            return;

        CurrentEpisode = previousEpisode;
    }

    private async Task UpdateCurrentEpisodeWatchedStatus(bool isWatched)
    {
        if (CurrentEpisode is null || Anime is null)
            return;

        using var session = _createSession();
        var anime = await session.GetAnimeAsync(Anime.Id);
        var watchedEpisode = anime.Episodes.FirstOrDefault(e => e.Position == CurrentEpisode.Position);
        if (watchedEpisode == null || watchedEpisode.IsWatched == isWatched)
            return;

        watchedEpisode.IsWatched = isWatched;
        await session.SaveChangesAsync();
        await LoadAnimeAsync(Anime.Id, true);
    }

    private async void AnimeSynchronizer_OnSynchronized(List<Anime> obj)
    {
        if (Anime is null)
            return;

        await Application.Current.Dispatcher.BeginInvoke(async () => await LoadAnimeAsync(Anime.Id, true));
    }

    private void Navigator_OnNavigated(object? view)
    {
        if (view is AnimePlayerView animePlayer && animePlayer.DataContext == this)
            return;

        _navigator.OnNavigated -= Navigator_OnNavigated;
        _animeSynchronizer.OnSynchronized -= AnimeSynchronizer_OnSynchronized;
    }
}