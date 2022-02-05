using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace AniCore.Core.AnimeProviders;

public static class AnimeToastHtmlExtensions
{
    public static string? ReadTitle(this IHtmlDocument document) =>
        document.GetElementsByClassName("entry-title")
                .FirstOrDefault()?.TextContent
                .Trim();

    public static IHtmlCollection<IElement>? ReadVoeLinks(this IHtmlDocument document)
    {
        var targetTabId = document.GetElementsByTagName("a")
                                  .FirstOrDefault(l => l.TextContent.Equals("voe", StringComparison.OrdinalIgnoreCase))?
                                  .GetAttribute("href")?[1..];

        return targetTabId != null 
            ? document.GetElementById(targetTabId)?.Children
            : null;
    }

    public static Uri? ReadPlayerLink(this IHtmlDocument document)
    {
        var link = document.GetElementById("player-embed")?
                           .Children.FirstOrDefault()?
                           .GetAttribute("href");

        return link != null ? new Uri(link) : null;
    }
}