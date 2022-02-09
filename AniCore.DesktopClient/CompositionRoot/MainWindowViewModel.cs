using AniCore.Core;
using AniCore.Core.DataAccess;
using AniCore.WpfClient.FrameworkExtensions;
using LightInject;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using AniCore.Core.AnimeSynchronization;
using AniCore.WpfClient.Animes;

namespace AniCore.WpfClient.CompositionRoot;

public sealed class MainWindowViewModel : BaseNotifyPropertyChanged, INavigator
{
    private const string WatchListKey = "Watchlist";
    private const string AnimesKey = "Animes";
    private const string AnimeImportKey = "Import Anime";

    private readonly IServiceFactory _container;
    private readonly IAnimeSynchronizer _animeSynchronizer;

    private object? _currentView;

    public MainWindowViewModel(IServiceFactory container, IAnimeSynchronizer animeSynchronizer)
    {
        _container = container;
        _animeSynchronizer = animeSynchronizer;
        RefreshAnimesCommand = new DelegateCommand(() => _animeSynchronizer.SynchronizeAsync());
        NavItemSelectedCommand = new ParameterizedCommand<NavigationItem?>(item =>
        {
            if (item is null)
                return;

            switch (item.Label)
            {
                case WatchListKey:
                    NavigateToWatchlist();
                    break;
                case AnimesKey:
                    NavigateToAnimes();
                    break;
                case AnimeImportKey:
                    NavigateToAnimeImport();
                    break;
            }
        });

        _animeSynchronizer.OnIsSynchronizingChanged += () => OnPropertyChanged(nameof(IsSynchronizing));
    }

    public ParameterizedCommand<NavigationItem?> NavItemSelectedCommand { get; }

    public List<INavigationItem> NavigationItems =>
        new ()
        {
            new FirstLevelNavigationItem { Label = WatchListKey, Icon = PackIconKind.Eye, IsSelected = true },
            new FirstLevelNavigationItem { Label = AnimesKey, Icon = PackIconKind.Video },
            new FirstLevelNavigationItem { Label = AnimeImportKey, Icon = PackIconKind.Import }
        };

    public object? CurrentView
    {
        get => _currentView;
        set => SetIfDifferent(ref _currentView, value);
    }

    public bool IsSynchronizing => _animeSynchronizer.IsSynchronizing;

    public DelegateCommand RefreshAnimesCommand { get; }

    public void NavigateToWatchlist() =>
        CurrentView = _container.GetInstance<WatchlistView>();

    public void NavigateToAnimes() =>
        CurrentView = _container.GetInstance<AnimesView>();

    public void NavigateToAnimeImport() =>
        CurrentView = _container.GetInstance<AnimeImportView>();

    public async void NavigateToPlayer(Anime anime)
    {
        var playerView = _container.GetInstance<AnimePlayerView>();
        var playerViewModel = (AnimePlayerViewModel) playerView.DataContext;
        await playerViewModel.LoadAnimeAsync(anime.Id);
        CurrentView = playerView;
    }
}