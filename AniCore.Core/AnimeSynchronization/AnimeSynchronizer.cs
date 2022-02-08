using AniCore.Core.AnimeProviders;
using AniCore.Core.DataAccess;

namespace AniCore.Core.AnimeSynchronization;

public sealed class AnimeSynchronizer : IAnimeSynchronizer
{
    private readonly Func<IAnimeSynchronizationSession> _createSession;
    private readonly Func<IAnimeProvider> _createAnimeProvider;
    private readonly System.Timers.Timer _syncTimer = new ()
    {
        Interval = TimeSpan.FromMinutes(5).TotalMilliseconds,
        AutoReset = true
    };

    private bool _isSynchronizing;

    public AnimeSynchronizer(Func<IAnimeSynchronizationSession> createSession, Func<IAnimeProvider> createAnimeProvider)
    {
        _createSession = createSession;
        _createAnimeProvider = createAnimeProvider;
    }

    public bool IsSynchronizing
    {
        get => _isSynchronizing;
        private set
        {
            _isSynchronizing = value;
            OnIsSynchronizingChanged?.Invoke();
        }
    }

    public event Action<List<Anime>>? OnSynchronized;

    public event Action? OnIsSynchronizingChanged;

    public async Task SynchronizeAsync()
    {
        if (IsSynchronizing)
            return;

        try
        {
            IsSynchronizing = true;

            using var animeProvider = _createAnimeProvider();
            using var session = _createSession();
            var animes = await session.GetAnimesAsync();

            var getActualAnimes = animes.Select(anime => animeProvider.ReadAnimeAsync(new Uri(anime.SourceUrl))).ToList();
            await Task.WhenAll(getActualAnimes);
            var actualAnimes = getActualAnimes.Where(t => t.Result is not null)
                                              .Select(t => t.Result!)
                                              .ToList();

            foreach (var anime in animes)
            {
                var actualAnime = actualAnimes.First(aa => aa.SourceUrl == anime.SourceUrl);
                anime.UpdateFrom(actualAnime);
            }

            await session.SaveChangesAsync();
            OnSynchronized?.Invoke(new List<Anime>());
        }
        finally
        {
            IsSynchronizing = false;
        }
    }

    public void Start()
    {
        Task.Run(SynchronizeAsync);
        _syncTimer.Start();
    }

    public void Stop() =>
        _syncTimer.Stop();
}