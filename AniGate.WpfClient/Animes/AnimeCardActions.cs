﻿using System;
using AniGate.Core;

namespace AniGate.WpfClient.Animes;

public sealed class AnimeCardActions
{
    public Action<Anime>? OnPlay { get; init; }

    public Action<Anime>? OnDelete { get; init; }

    public Action<Anime>? OnRemoveFromWatchlist { get; init; }

    public Action<Anime>? OnAddToWatchlist { get; init; }
}