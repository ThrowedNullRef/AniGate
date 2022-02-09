namespace AniGate.Core.DataAccess;

public interface IAnimePlayerSession : IUnitOfWorkSession
{
    Task<Anime> GetAnimeAsync(string id);
}