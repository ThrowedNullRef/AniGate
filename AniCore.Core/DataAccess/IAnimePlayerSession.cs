namespace AniCore.Core.DataAccess;

public interface IAnimePlayerSession : IUnitOfWorkSession
{
    Task<Anime> GetAnimeAsync(string id);
}