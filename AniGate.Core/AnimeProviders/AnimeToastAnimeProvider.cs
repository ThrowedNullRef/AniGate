using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace AniGate.Core.AnimeProviders;

public sealed class AnimeToastAnimeProvider : IAnimeProvider
{
    private readonly Dictionary<string, IHtmlDocument?> _htmlDocumentsByUrl = new ();
    private readonly HttpClient _httpClient = new ();
    private readonly HtmlParser _htmlParser = new ();

    private async Task<IHtmlDocument?> ReadHtmlDocumentAsync(Uri url)
    {
        if (_htmlDocumentsByUrl.TryGetValue(url.ToString(), out var cachedHtml))
            return cachedHtml;

        try
        {
            var response = await _httpClient.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            var document = _htmlParser.ParseDocument(html);
            _htmlDocumentsByUrl[url.ToString()] = document;
            return document;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Anime?> ReadAnimeAsync(Uri url)
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
            Episodes = await ReadEpisodesAsync(url),
            Thumbnail = thumbnailUrl is not null ? await _httpClient.GetByteArrayAsync(thumbnailUrl) : null
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

    public void Dispose() =>
        _httpClient.Dispose();
}