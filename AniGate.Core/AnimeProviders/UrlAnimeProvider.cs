using System.Collections.Concurrent;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace AniGate.Core.AnimeProviders;

public abstract class UrlAnimeProvider : IUrlAnimeProvider
{
    private readonly HtmlParser _htmlParser = new();
    private readonly ConcurrentDictionary<string, IHtmlDocument> _htmlDocumentsByUrl = new ();

    protected UrlAnimeProvider(string sourceName, HttpClient httpClient)
    {
        ServiceProvider = sourceName;
        HttpClient = httpClient;
    }

    public string ServiceProvider { get; }

    protected HttpClient HttpClient { get; }

    public abstract Task<Anime?> ReadAnimeAsync(Uri url);

    public void Dispose() =>
        HttpClient.Dispose();

    protected async Task<IHtmlDocument?> ReadHtmlDocumentAsync(Uri url)
    {
        if (_htmlDocumentsByUrl.TryGetValue(url.AbsoluteUri, out var document))
            return document;

        try
        {
            var response = await HttpClient.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            document = _htmlParser.ParseDocument(html);
            _htmlDocumentsByUrl[url.AbsolutePath] = document;
            return document;
        }
        catch
        {
            return null;
        }
    }
}