namespace AniGate.Core.DataAccess;

public interface IAnimeImportSession : IUnitOfWorkSession
{
    Task AddAnimeAsync(Anime anime);
}