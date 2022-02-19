using AniGate.Core;
using AniGate.WpfClient.FrameworkExtensions;

namespace AniGate.WpfClient.Animes;

public sealed class AnimeCardViewModel : BaseNotifyPropertyChanged
{
    private int _imageBorderThickness;

    public AnimeCardViewModel(Anime anime, AnimeCardActions actions)
    {
        Anime = anime;

        var commandsCount = 0;
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

        PlayCommand = new DelegateCommand(() => actions.OnPlay(anime));
        MouseEnterCommand = new DelegateCommand(Focus);
        MouseLeaveCommand = new DelegateCommand(UndoFocus);
        CommandsCount = commandsCount;
        HasUnwatchedEpisodes = anime.HasUnwatchedEpisode();
    }

    public Anime Anime { get; }

    public bool HasUnwatchedEpisodes { get; }

    public DelegateCommand? PlayCommand { get; }

    public DelegateCommand? DeleteCommand { get; }

    public DelegateCommand? AddToWatchlistCommand { get; }

    public DelegateCommand? RemoveFromWatchlistCommand { get; }

    public DelegateCommand MouseEnterCommand { get; }

    public DelegateCommand MouseLeaveCommand { get; }

    public int CommandsCount { get; }

    public int ImageBorderThickness
    {
        get => _imageBorderThickness;
        set => SetIfDifferent(ref _imageBorderThickness, value);
    }

    private void Focus()
    {
        ImageBorderThickness = 12;
    }

    private void UndoFocus()
    {
        ImageBorderThickness = 0;
    }
}