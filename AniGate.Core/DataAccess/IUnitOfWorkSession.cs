namespace AniGate.Core.DataAccess;

public interface IUnitOfWorkSession : IDisposable
{
    Task SaveChangesAsync();
}