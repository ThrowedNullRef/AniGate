using AniGate.Core;
using AniGate.Core.DataAccess;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace AniGate.RavenDataAccess.Sessions;

public sealed class RavenAnimesSession : RavenUnitOfWork, IAnimesSession
{
    public RavenAnimesSession(IAsyncDocumentSession session) : base(session)
    {
    }

    public Task<List<Anime>> GetAnimesAsync() =>
        Session.Query<Anime>()
               .OrderBy(anime => anime.OriginalName)
               .ToListAsync();

    public Task<Anime> GetAnimeAsync(string id) =>
        Session.LoadAsync<Anime>(id);

    public void DeleteAnime(Anime anime) =>
        Session.Delete(anime);
}