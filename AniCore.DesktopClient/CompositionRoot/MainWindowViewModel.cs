using AniCore.Core.AnimeProviders;
using AniCore.Core.DataAccess;
using AniCore.DesktopClient.FrameworkExtensions;
using AniCore.DesktopClient.Watchlist;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AniCore.DesktopClient.CompositionRoot;

public sealed class MainWindowViewModel : BaseViewModel
{
    private readonly Func<IWatchlistSession> _createSession;
    private readonly Func<IAnimeProvider> _createAnimeProvider;

    private bool _isAddingAnimeToWatchlist;
    private string _newAnimeUrl = string.Empty;
    private List<WatchlistEpisode> _episodes = new();
    private bool _isLoading;

    public MainWindowViewModel(Func<IWatchlistSession> createSession, Func<IAnimeProvider> createAnimeProvider)
    {
        _createSession = createSession;
        _createAnimeProvider = createAnimeProvider;
        AddToWatchlistCommand = new DelegateCommand(AddAnimeToWatchlist, () => Uri.TryCreate(NewAnimeUrl, UriKind.Absolute, out _));
        WatchEpisodeCommand = new ParameterizedCommand<WatchlistEpisode>(WatchEpisode);
    }

    public List<WatchlistEpisode> Episodes
    {
        get => _episodes;
        set
        {
            _episodes = value.OrderByDescending(e => e.Episode.IsNew)
                             .ThenBy(e => e.Anime.OriginalName)
                             .ThenBy(e => e.Episode.Position)
                             .ToList();
            OnPropertyChanged(nameof(Episodes));
        }
    }

    public DelegateCommand AddToWatchlistCommand { get; }

    public ParameterizedCommand<WatchlistEpisode> WatchEpisodeCommand { get; }

    public bool IsAddingAnimeToWatchlist
    {
        get => _isAddingAnimeToWatchlist;
        private set
        {
            _isAddingAnimeToWatchlist = value;
            OnPropertyChanged(nameof(IsAddingAnimeToWatchlist));
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }


    public string NewAnimeUrl
    {
        get => _newAnimeUrl;
        set
        {
            _newAnimeUrl = value;
            AddToWatchlistCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(NewAnimeUrl));
        }
    }

    public async void WatchEpisode(WatchlistEpisode? episode)
    {
        if (episode is null)
            return;

        Process.Start(new ProcessStartInfo(episode.Episode.VoeLink)
        {
            UseShellExecute = true
        });

        using var session = _createSession();
        var anime = await session.GetAnimeAsync(episode.Anime.Id);
        var watchedEpisode = anime.Episodes.FirstOrDefault(e => e.Position == episode.Episode.Position);
        if (watchedEpisode == null) 
            return;

        watchedEpisode.IsNew = false;
        await session.SaveChangesAsync();
        await ReloadAnimesAsync();
    }

    public async void AddAnimeToWatchlist()
    {
        if (IsAddingAnimeToWatchlist || !Uri.TryCreate(NewAnimeUrl, UriKind.Absolute, out var animeUri))
            return;

        try
        {
            IsAddingAnimeToWatchlist = true;
            using var animeProvider = _createAnimeProvider();
            var anime = await animeProvider.ReadAnimeAsync(animeUri);

            using var session = _createSession();
            await session.AddAnimeAsync(anime);
            await session.SaveChangesAsync();

            Episodes = anime.Episodes
                            .Select(e => new WatchlistEpisode(anime, e))
                            .Concat(Episodes)
                            .ToList();

            NewAnimeUrl = string.Empty;
        }
        finally
        {
            IsAddingAnimeToWatchlist = false;
        }
    }

    public async Task ReloadAnimesAsync()
    {
        if (IsLoading)
            return;

        try
        {
            IsLoading = true;
            using var session = _createSession();
            var animes = await session.GetAnimesAsync();
            Episodes = animes.SelectMany(anime => anime.Episodes.Select(episode => new WatchlistEpisode(anime, episode)))
                             .ToList();
            OnPropertyChanged(nameof(Episodes));
        }
        finally
        {
            IsLoading = false;
        }
    }
}