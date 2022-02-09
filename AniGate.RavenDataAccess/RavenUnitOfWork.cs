using AniGate.Core.DataAccess;
using Raven.Client.Documents.Session;

namespace AniGate.RavenDataAccess;

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