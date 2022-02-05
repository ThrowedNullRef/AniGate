using AniCore.Core.DataAccess;
using Raven.Client.Documents.Session;

namespace AniCore.RavenDataAccess;

public abstract class RavenUnitOfWork : IAsyncUnitOfWork
{
    protected RavenUnitOfWork(IAsyncDocumentSession session)
    {
        Session = session;
    }

    protected IAsyncDocumentSession Session { get; }

    public Task SaveChangesAsync() => Session.SaveChangesAsync();

    public void Dispose() => Session.Dispose();
}