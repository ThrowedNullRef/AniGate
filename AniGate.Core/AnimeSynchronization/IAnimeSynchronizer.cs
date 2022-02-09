namespace AniGate.Core.AnimeSynchronization;

public interface IAnimeSynchronizer : IHostedService
{
    bool IsSynchronizing { get; }

    Task SynchronizeAsync();

    event Action OnIsSynchronizingChanged;

    event Action<List<Anime>> OnSynchronized;
}