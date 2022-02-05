using System;
using AniCore.Core;

namespace AniCore.DesktopClient.Watchlist
{
    public sealed class WatchlistEpisode
    {
        public WatchlistEpisode(Anime anime, Episode episode)
        {
            Anime = anime;
            Episode = episode;
        }

        public Anime Anime { get; }

        public Episode Episode { get; }

        public string EpisodeNumber => $"Ep. {Episode.Position + 1}";
    }
}
