using AniGate.Core;
using AniGate.Core.DataAccess;
using Raven.Client.Documents.Session;

namespace AniGate.RavenDataAccess.Sessions;

public sealed class RavenAnimePlayerSession : RavenUnitOfWork, IAnimePlayerSession
{
    public RavenAnimePlayerSession(IAsyncDocumentSession session) : base(session)
    {
    }

    public Task<Anime> GetAnimeAsync(string id) =>
        Session.LoadAsync<Anime>(id);
}