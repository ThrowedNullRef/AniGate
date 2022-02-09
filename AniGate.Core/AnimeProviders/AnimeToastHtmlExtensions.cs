using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AniGate.Core.FrameworkExtensions;

namespace AniGate.Core.AnimeProviders;

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

    public static Uri? ReadPlayerLink(this IHtmlDocument document) =>
        document.GetElementById("player-embed")?
                .Children.FirstOrDefault()?
                .GetAttribute("href")
                .ToUri();

    public static Uri? ReadThumbnailUrl(this IHtmlDocument document) =>
        document.GetElementsByClassName("size-full alignleft")
                .FirstOrDefault()?
                .GetAttribute("src")?
                .ToUri();
}