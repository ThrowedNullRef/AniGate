using System;
using AniGate.Core;

namespace AniGate.WpfClient.Animes;

public sealed class AnimeCardActions
{
    public AnimeCardActions(Action<Anime> onPlay)
    {
        OnPlay = onPlay;
    }

    public Action<Anime> OnPlay { get; }

    public Action<Anime>? OnDelete { get; init; }

    public Action<Anime>? OnRemoveFromWatchlist { get; init; }

    public Action<Anime>? OnAddToWatchlist { get; init; }
}