namespace AniCore.Core;

public sealed class Episode
{
    public int Position { get; init; } = -1;

    public string VoeLink { get; init; } = string.Empty;

    public bool IsNew { get; set; } = true;
}