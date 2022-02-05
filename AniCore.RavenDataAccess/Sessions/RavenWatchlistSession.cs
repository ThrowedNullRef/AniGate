using AniCore.Core;
using AniCore.Core.DataAccess;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace AniCore.RavenDataAccess.Sessions;

public sealed class RavenWatchlistSession : RavenUnitOfWork, IWatchlistSession
{
    public RavenWatchlistSession(IAsyncDocumentSession session) : base(session)
    {
    }

    public Task<List<Anime>> GetAnimesAsync() =>
        Session.Query<Anime>()
               .ToListAsync();

    public Task AddAnimeAsync(Anime anime) =>
        Session.StoreAsync(anime);

    public Task<Anime> GetAnimeAsync(string id) =>
        Session.LoadAsync<Anime>(id);
}