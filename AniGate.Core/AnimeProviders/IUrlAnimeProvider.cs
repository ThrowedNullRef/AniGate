namespace AniGate.Core.AnimeProviders;

public interface IUrlAnimeProvider : IDisposable
{
    string ServiceProvider { get; }

    Task<Anime?> ReadAnimeAsync(Uri url);
}