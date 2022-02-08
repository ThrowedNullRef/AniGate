using System;
using AniCore.Core.AnimeProviders;
using AniCore.Core.DataAccess;
using AniCore.WpfClient.FrameworkExtensions;

namespace AniCore.WpfClient.Animes;

public sealed class AnimeImportViewModel : BaseNotifyPropertyChanged
{
    private readonly Func<IAnimeImportSession> _createSession;
    private readonly Func<IAnimeProvider> _createAnimeProvider;

    private string _animeUrl = string.Empty;
    private bool _isImporting;

    public AnimeImportViewModel(Func<IAnimeImportSession> createSession, Func<IAnimeProvider> createAnimeProvider)
    {
        _createSession = createSession;
        _createAnimeProvider = createAnimeProvider;
        ImportCommand = new DelegateCommand(ImportAnime, IsValidUrl);
    }

    public DelegateCommand ImportCommand { get; }

    public string AnimeUrl
    {
        get => _animeUrl;
        set
        {
            SetIfDifferent(ref _animeUrl, value);
            ImportCommand.RaiseCanExecuteChanged();
        }
    }

    public bool ImportAnimeAsWatching { get; set; } = true;

    public bool IsImporting
    {
        get => _isImporting;
        set => SetIfDifferent(ref _isImporting, value);
    }

    private async void ImportAnime()
    {
        if (IsImporting || !Uri.TryCreate(AnimeUrl, UriKind.Absolute, out var parseUri))
            return;

        try
        {
            IsImporting = false;
            using var animeProvider = _createAnimeProvider();
            var anime = await animeProvider.ReadAnimeAsync(parseUri);
            if (anime is null)
                return;

            if (ImportAnimeAsWatching)
                anime.IsWatching = true;

            using var session = _createSession();
            await session.AddAnimeAsync(anime);
            await session.SaveChangesAsync();
            AnimeUrl = string.Empty;
        }
        finally
        {
            IsImporting = true;
        }
    }

    private bool IsValidUrl() =>
        Uri.TryCreate(AnimeUrl, UriKind.Absolute, out _);
}