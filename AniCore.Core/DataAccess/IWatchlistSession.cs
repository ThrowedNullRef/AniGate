namespace AniCore.Core.DataAccess
{
    public interface IWatchlistSession : IUnitOfWorkSession
    {
        Task<List<Anime>> GetAnimesAsync();

        Task AddAnimeAsync(Anime anime);

        Task<Anime> GetAnimeAsync(string id);
    }
}
