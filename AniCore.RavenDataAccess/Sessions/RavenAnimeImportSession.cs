using AniCore.Core;
using AniCore.Core.DataAccess;
using Raven.Client.Documents.Session;

namespace AniCore.RavenDataAccess.Sessions;

public sealed class RavenAnimeImportSession : RavenUnitOfWork, IAnimeImportSession
{
    public RavenAnimeImportSession(IAsyncDocumentSession session) : base(session)
    {
    }

    public Task AddAnimeAsync(Anime anime) =>
        Session.StoreAsync(anime);
}