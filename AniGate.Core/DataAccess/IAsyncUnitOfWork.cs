namespace AniGate.Core.DataAccess;

public interface IAsyncUnitOfWork : IDisposable
{
    Task SaveChangesAsync();
}