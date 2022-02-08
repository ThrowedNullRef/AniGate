using AniCore.Core;
using AniCore.Core.DataAccess;
using Raven.Client.Documents.Session;

namespace AniCore.RavenDataAccess.Sessions;

public sealed class RavenAnimePlayerSession : RavenUnitOfWork, IAnimePlayerSession
{
    public RavenAnimePlayerSession(IAsyncDocumentSession session) : base(session)
    {
    }

    public Task<Anime> GetAnimeAsync(string id) =>
        Session.LoadAsync<Anime>(id);
}