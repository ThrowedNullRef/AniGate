using AniGate.Core.AnimeProviders;
using AniGate.Core.DataAccess;

namespace AniGate.Core.AnimeSynchronization;

public sealed class AnimeSynchronizer : IAnimeSynchronizer
{
    private readonly Func<IAnimeSynchronizationSession> _createSession;
    private readonly Func<IUrlAnimeProvider> _createAnimeProvider;
    private readonly System.Timers.Timer _syncTimer = new ()
    {
        Interval = TimeSpan.FromSeconds(60).TotalMilliseconds,
        AutoReset = true
    };

    private bool _isSynchronizing;

    public AnimeSynchronizer(Func<IAnimeSynchronizationSession> createSession, Func<IUrlAnimeProvider> createAnimeProvider)
    {
        _createSession = createSession;
        _createAnimeProvider = createAnimeProvider;
        _syncTimer.Elapsed += async (_, __) => await SynchronizeAsync();
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
                var actualAnime = actualAnimes.FirstOrDefault(aa => aa.SourceUrl == anime.SourceUrl);
                if (actualAnime is not null)
                {
                    anime.UpdateFrom(actualAnime);
                }
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