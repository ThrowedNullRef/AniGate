using AniGate.Core;
using AniGate.WpfClient.FrameworkExtensions;

namespace AniGate.WpfClient.Animes;

public sealed class AnimeCardViewModel
{
    public AnimeCardViewModel(Anime anime, AnimeCardActions actions)
    {
        Anime = anime;
        NewEpisodePrefix = anime.HasUnwatchedEpisode() ? "Neue Folge(n):" : string.Empty;

        var commandsCount = 0;
        if (actions.OnPlay is not null)
        {
            PlayCommand = new DelegateCommand(() => actions.OnPlay(anime));
            ++commandsCount;
        }

        if (actions.OnDelete is not null)
        {
            DeleteCommand = new DelegateCommand(() => actions.OnDelete(anime));
            ++commandsCount;
        }

        if (actions.OnAddToWatchlist is not null && !anime.IsWatching)
        {
            AddToWatchlistCommand = new DelegateCommand(() => actions.OnAddToWatchlist(anime));
            ++commandsCount;
        }

        if (actions.OnRemoveFromWatchlist is not null && anime.IsWatching)
        {
            RemoveFromWatchlistCommand = new DelegateCommand(() => actions.OnRemoveFromWatchlist(anime));
            ++commandsCount;
        }

        CommandsCount = commandsCount;
    }

    public Anime Anime { get; }

    public string NewEpisodePrefix { get; }

    public DelegateCommand? PlayCommand { get; }

    public DelegateCommand? DeleteCommand { get; }

    public DelegateCommand? AddToWatchlistCommand { get; }

    public DelegateCommand? RemoveFromWatchlistCommand { get; }

    public int CommandsCount { get; }
}