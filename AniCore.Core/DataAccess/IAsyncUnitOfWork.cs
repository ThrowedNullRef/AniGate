namespace AniCore.Core.DataAccess;

public interface IAsyncUnitOfWork : IDisposable
{
    Task SaveChangesAsync();
}