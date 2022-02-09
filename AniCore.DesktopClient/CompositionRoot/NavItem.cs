using System.Collections.Generic;
using MaterialDesignThemes.Wpf;

namespace AniCore.WpfClient.CompositionRoot;

public sealed class NavigationTarget
{
    public const string WatchListKey = "Watchlist";
    public const string AnimesKey = "Animes";
    public const string AnimeImportKey = "Import Anime";

    public static List<NavigationTarget> All = new ()
    {
        new (WatchListKey, "Watchlist", PackIconKind.Eye),
        new (AnimesKey, "Animes", PackIconKind.Movie),
        new (AnimeImportKey, "Import Anime", PackIconKind.Download)
    };

    public NavigationTarget(string key, string label, PackIconKind icon)
    {
        Key = key;
        Label = label;
        Icon = icon;
    }

    public string Key { get; }

    public string Label { get; }

    public PackIconKind Icon { get; }
}