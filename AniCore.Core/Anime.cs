namespace AniCore.Core;

public sealed class Anime : GuidEntity
{
    public string OriginalName { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string SourceUrl { get; init; } = string.Empty;

    public List<Episode> Episodes { get; init; } = new ();
}
