namespace AniCore.Core.DataAccess;

public interface IAnimesSession : IUnitOfWorkSession
{
    Task<List<Anime>> GetAnimesAsync();

    Task<Anime> GetAnimeAsync(string id);

    void DeleteAnime(Anime anime);
}