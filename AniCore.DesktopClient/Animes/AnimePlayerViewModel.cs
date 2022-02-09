using AniCore.Core;
using AniCore.Core.AnimeSynchronization;
using AniCore.Core.DataAccess;
using AniCore.WpfClient.Common;
using AniCore.WpfClient.FrameworkExtensions;
using MaterialDesignExtensions.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AniCore.WpfClient.Animes;

public sealed class AnimePlayerViewModel : BaseNotifyPropertyChanged
{
    private readonly Func<IAnimePlayerSession> _createSession;
    private readonly IAnimeSynchronizer _animeSynchronizer;
    private List<INavigationItem> _episodeItems = new ();
    private Episode? _currentEpisode;
    private WebPlayerViewModel? _webPlayerViewModel;

    public AnimePlayerViewModel(Func<IAnimePlayerSession> createSession, IAnimeSynchronizer animeSynchronizer)
    {
        _createSession = createSession;
        _animeSynchronizer = animeSynchronizer;
        NavItemSelectedCommand = new ParameterizedCommand<NavigationItem>(OnNavItemSelected);
        animeSynchronizer.OnSynchronized += AnimeSynchronizer_OnSynchronized;
    }

    ~AnimePlayerViewModel()
    {
        _animeSynchronizer.OnSynchronized -= AnimeSynchronizer_OnSynchronized;
    }

    public Anime? Anime { get; private set; }

    public WebPlayerViewModel? WebPlayerViewModel
    {
        get => _webPlayerViewModel;
        set => SetIfDifferent(ref _webPlayerViewModel, value);
    }

    public List<INavigationItem> EpisodeItems
    {
        get => _episodeItems;
        private set => SetIfDifferent(ref _episodeItems, value);
    }

    public Episode? CurrentEpisode
    {
        get => _currentEpisode;
        set
        {
            SetIfDifferent(ref _currentEpisode, value);
            WebPlayerViewModel = value is not null ? new WebPlayerViewModel(new Uri(value.VoeLink)) : null;
        }
    }

    public ParameterizedCommand<NavigationItem> NavItemSelectedCommand { get; }

    public bool OpenInBrowser { get; set; }

    public async Task LoadAnimeAsync(string animeId)
    {
        using var session = _createSession();
        CurrentEpisode = null;
        Anime = await session.GetAnimeAsync(animeId);
        var orderedEpisodes = Anime.Episodes.OrderBy(e => e.Position).ToList();
        EpisodeItems = orderedEpisodes.Select(episode => ConvertEpisodeToNavItem(episode, false)).ToList();
    }

    private async void OnNavItemSelected(NavigationItem? item)
    {
        if (Anime is null || item is null)
            return;

        CurrentEpisode = Anime.Episodes.First(e => e.ToString() == item.Label);

        if (OpenInBrowser)
        {
            OpenCurrentEpisodeInBrowser(Anime.Id);
        }

        await MarkEpisodeAsWatchedAsync(Anime.Id, CurrentEpisode.Position);
    }

    private void OpenCurrentEpisodeInBrowser(string animeId)
    {
        if (CurrentEpisode is null)
            return;

        Process.Start(new ProcessStartInfo(CurrentEpisode.VoeLink)
        {
            UseShellExecute = true
        });
    }

    private async Task MarkEpisodeAsWatchedAsync(string animeId, int episodePos)
    {
        using var session = _createSession();
        var anime = await session.GetAnimeAsync(animeId);
        var watchedEpisode = anime.Episodes.FirstOrDefault(e => e.Position == episodePos);
        if (watchedEpisode == null || watchedEpisode.IsWatched)
            return;

        watchedEpisode.IsWatched = true;
        await session.SaveChangesAsync();
    }

    private void AnimeSynchronizer_OnSynchronized(List<Anime> obj)
    {
        if (Anime is null)
            return;

        //await LoadAnimeAsync(Anime.Id);
    }

    private static INavigationItem ConvertEpisodeToNavItem(Episode episode, bool isSelected) =>
       new FirstLevelNavigationItem
       {
           Label = episode.ToString(),
           IsSelected = isSelected
       };
}