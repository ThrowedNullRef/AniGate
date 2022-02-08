namespace AniCore.Core;

public sealed class Episode
{
    public int Position { get; init; } = -1;

    public string VoeLink { get; set; } = string.Empty;

    public bool IsWatched { get; set; } = false;

    public override string ToString() => $"Ep. {Position + 1}";

    public void UpdateFrom(Episode episode)
    {
        VoeLink = episode.VoeLink;
    }
}