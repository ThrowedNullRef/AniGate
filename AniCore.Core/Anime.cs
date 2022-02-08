namespace AniCore.Core;

public sealed class Anime : GuidEntity
{
    public string OriginalName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string SourceUrl { get; init; } = string.Empty;

    public bool IsWatching { get; set; }

    public byte[]? Thumbnail { get; set; }

    public List<Episode> Episodes { get; set; } = new ();

    public override string ToString() => OriginalName;

    public bool HasUnwatchedEpisode() => Episodes.Any(e => !e.IsWatched);

    public void UpdateFrom(Anime anime)
    {
        OriginalName = anime.OriginalName;
        Name = anime.Name;
        Thumbnail = anime.Thumbnail;

        foreach (var episode in anime.Episodes)
        {
            var foundEpisode = Episodes.FirstOrDefault(e => e.Position == episode.Position);
            if (foundEpisode == null)
            {
                Episodes.Add(episode);
            }
            else
            {
                foundEpisode.UpdateFrom(episode);
            }
        }
    }
}
