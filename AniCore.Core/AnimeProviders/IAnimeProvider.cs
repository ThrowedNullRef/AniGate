namespace AniCore.Core.AnimeProviders;

public interface IAnimeProvider : IDisposable
{
    Task<Anime> ReadAnimeAsync(Uri url);
}