namespace AniCore.Core.DataAccess;

public interface IUnitOfWorkSession : IDisposable
{
    Task SaveChangesAsync();
}