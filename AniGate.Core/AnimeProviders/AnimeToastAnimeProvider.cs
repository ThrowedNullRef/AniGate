﻿namespace AniGate.Core.AnimeProviders;

public sealed class AnimeToastAnimeProvider : UrlAnimeProvider
{
    public AnimeToastAnimeProvider() : base("AnimeToast", new HttpClient())
    {
    }

    public override async Task<Anime?> ReadAnimeAsync(Uri url)
    {
        var document = await ReadHtmlDocumentAsync(url);
        if (document is null)
            return null;

        var title = document.ReadTitle() ?? "[Unknown]";
        var thumbnailUrl = document.ReadThumbnailUrl();

        var anime = new Anime
        {
            OriginalName = title,
            Name = title,
            SourceUrl = url.ToString(),
            ServiceProvider = ServiceProvider,
            Episodes = await ReadEpisodesAsync(url),
            Thumbnail = thumbnailUrl is not null ? await HttpClient.GetByteArrayAsync(thumbnailUrl) : null
        };
        anime.SetNewGuid();
        return anime;
    }

    private async Task<List<Episode>> ReadEpisodesAsync(Uri url)
    {
        var document = await ReadHtmlDocumentAsync(url);
        var voeElements = document?.ReadVoeLinks()?.ToList();

        if (voeElements == null || voeElements.Count < 1)
            return new ();

        if (voeElements.Count == 1)
        {
            var firstVoeElement = voeElements.First();
            if (firstVoeElement.TextContent.Contains('-'))
            {
                var vowGroupLink = firstVoeElement.GetAttribute("href");
                var voeGroupDoc = await ReadHtmlDocumentAsync(new Uri(vowGroupLink!));
                if (voeGroupDoc is null)
                    return new ();

                var linkToEpisodes = voeGroupDoc.ReadPlayerLink();
                return await ReadEpisodesAsync(linkToEpisodes!);
            }
        }

        var tasks = new List<Task<Episode?>>();
        for (var i = 0; i < voeElements.Count; ++i)
        {
            var episodeUrl = new Uri(voeElements[i].GetAttribute("href")!);
            tasks.Add(ReadEpisodeAsync(episodeUrl, i));
        }

        await Task.WhenAll(tasks);

        return tasks.Where(t => t.Result is not null)
                    .Select(t => t.Result!)
                    .ToList();
    }

    private async Task<Episode?> ReadEpisodeAsync(Uri episodeUrl, int position)
    {
        var episodeDocument = await ReadHtmlDocumentAsync(episodeUrl);
        var voeLink = episodeDocument?.ReadPlayerLink();
        if (voeLink is null)
            return null;

        var url = $"{voeLink.Scheme}://{voeLink.Host}/e{voeLink.PathAndQuery}";
        voeLink = new Uri(url);

        return new Episode
        {
            Position = position,
            VoeLink = voeLink.ToString(),
        };
    }
}