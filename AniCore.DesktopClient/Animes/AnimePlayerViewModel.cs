using AniCore.Core;
using AniCore.Core.AnimeSynchronization;
using AniCore.Core.DataAccess;
using AniCore.WpfClient.CompositionRoot;
using AniCore.WpfClient.FrameworkExtensions;
using MaterialDesignExtensions.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AniCore.WpfClient.Animes;

public sealed class AnimePlayerViewModel : BaseNotifyPropertyChanged
{
    private readonly Func<IAnimePlayerSession> _createSession;
    private readonly IAnimeSynchronizer _animeSynchronizer;
    private readonly MainWindow _mainWindow;
    private List<INavigationItem> _episodeItems = new ();
    private Episode? _currentEpisode;
    private bool _isFullScreen;
    private double _mainMargin = 40;
    private int _playerColumnSpan = 1;
    private int _playerColumn = 1;
    private int _playerRowSpan = 1;
    private int _playerRow = 1;

    public AnimePlayerViewModel(Func<IAnimePlayerSession> createSession, IAnimeSynchronizer animeSynchronizer, MainWindow mainWindow)
    {
        _createSession = createSession;
        _animeSynchronizer = animeSynchronizer;
        _mainWindow = mainWindow;
        NavItemSelectedCommand = new ParameterizedCommand<NavigationItem>(OnNavItemSelected);
        animeSynchronizer.OnSynchronized += AnimeSynchronizer_OnSynchronized;
    }

    ~AnimePlayerViewModel()
    {
        _animeSynchronizer.OnSynchronized -= AnimeSynchronizer_OnSynchronized;
    }

    public Anime? Anime { get; private set; }

    public List<INavigationItem> EpisodeItems
    {
        get => _episodeItems;
        private set => SetIfDifferent(ref _episodeItems, value);
    }

    public Episode? CurrentEpisode
    {
        get => _currentEpisode;
        set => SetIfDifferent(ref _currentEpisode, value);
    }

    public ParameterizedCommand<NavigationItem> NavItemSelectedCommand { get; }

    public double MainMargin
    {
        get => _mainMargin;
        set => SetIfDifferent(ref _mainMargin, value);
    }

    public int PlayerColumnSpan
    {
        get => _playerColumnSpan;
        set => SetIfDifferent(ref _playerColumnSpan, value);
    }

    public int PlayerColumn
    {
        get => _playerColumn;
        set => SetIfDifferent(ref _playerColumn, value);
    }

    public int PlayerRow
    {
        get => _playerRow;
        set => SetIfDifferent(ref _playerRow, value);
    }

    public int PlayerRowSpan
    {
        get => _playerRowSpan;
        set => SetIfDifferent(ref _playerRowSpan, value);
    }

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

    private async void AnimeSynchronizer_OnSynchronized(List<Anime> obj)
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

    public void ToggleFullScreen()
    {
        var isFullScreen = !_isFullScreen;
        MainMargin = isFullScreen ? 00.0 : 40.0;
        PlayerColumn = isFullScreen ? 0 : 1;
        PlayerColumnSpan = isFullScreen ? 2 : 1;
        PlayerRow = isFullScreen ? 0 : 1;
        PlayerRowSpan = isFullScreen ? 2 : 1;
        _mainWindow.WindowState = isFullScreen ? WindowState.Maximized: WindowState.Normal;
        ((MainWindowViewModel)_mainWindow.DataContext).IsContentFullScreen = isFullScreen;
        _isFullScreen = isFullScreen;
    }
}