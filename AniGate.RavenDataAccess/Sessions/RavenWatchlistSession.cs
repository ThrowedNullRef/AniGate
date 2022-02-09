using AniGate.Core;
using AniGate.Core.DataAccess;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace AniGate.RavenDataAccess.Sessions;

public sealed class RavenWatchlistSession : RavenUnitOfWork, IWatchlistSession
{
    public RavenWatchlistSession(IAsyncDocumentSession session) : base(session)
    {
    }

    public Task<List<Anime>> GetWatchlistAnimesAsync() =>
        Session.Query<Anime>()
               .Where(a => a.IsWatching)
               .ToListAsync();

    public Task<Anime> GetAnimeAsync(string id) =>
        Session.LoadAsync<Anime>(id);
}