namespace AniCore.Core.DataAccess
{
    public interface IWatchlistSession : IUnitOfWorkSession
    {
        Task<List<Anime>> GetWatchlistAnimesAsync();

        Task<Anime> GetAnimeAsync(string id);
    }
}
