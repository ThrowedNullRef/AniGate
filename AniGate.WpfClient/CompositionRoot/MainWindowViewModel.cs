using System.Collections.Generic;
using System.Linq;
using AniGate.Core;
using AniGate.Core.AnimeSynchronization;
using AniGate.WpfClient.Animes;
using AniGate.WpfClient.FrameworkExtensions;
using LightInject;
using MaterialDesignExtensions.Model;

namespace AniGate.WpfClient.CompositionRoot;

public sealed class MainWindowViewModel : BaseNotifyPropertyChanged, INavigator
{
    private readonly IServiceFactory _container;
    private readonly IAnimeSynchronizer _animeSynchronizer;

    private object? _currentView;
    private string? _previousViewKey;

    public MainWindowViewModel(IServiceFactory container, IAnimeSynchronizer animeSynchronizer)
    {
        _container = container;
        _animeSynchronizer = animeSynchronizer;
        RefreshAnimesCommand = new DelegateCommand(() => _animeSynchronizer.SynchronizeAsync());

        NavItemSelectedCommand = new ParameterizedCommand<NavigationItem?>(item =>
        {
            if (item is null)
                return;

            NavigateByKey(item.Label);
        });

        _animeSynchronizer.OnIsSynchronizingChanged += () => OnPropertyChanged(nameof(IsSynchronizing));
    }

    public ParameterizedCommand<NavigationItem?> NavItemSelectedCommand { get; }

    public List<INavigationItem> NavigationItems => 
        NavigationTarget.All
                        .Select((nt, index) =>
                         {
                             var item = new FirstLevelNavigationItem
                             {
                                 Label = nt.Key,
                                 Icon = nt.Icon,
                                 IsSelected = index == 0
                             };

                             return (INavigationItem)item;
                         })
                        .ToList();

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

    public void NavigateToAnimeImport()
    {
        CurrentView = _container.GetInstance<AnimeImportView>();
    }

    public async void NavigateToPlayer(Anime anime)
    {
        var playerView = _container.GetInstance<AnimePlayerView>();
        var playerViewModel = (AnimePlayerViewModel) playerView.DataContext;
        await playerViewModel.LoadAnimeAsync(anime.Id);
        CurrentView = playerView;
    }

    public void NavigateBack()
    {
        if (_previousViewKey is null)
            return;

        NavigateByKey(_previousViewKey);
        _previousViewKey = null;
    }

    private void NavigateByKey(string key)
    {
        switch (key)
        {
            case NavigationTarget.WatchListKey:
                NavigateToWatchlist();
                break;
            case NavigationTarget.AnimesKey:
                NavigateToAnimes();
                break;
            case NavigationTarget.AnimeImportKey:
                NavigateToAnimeImport();
                break;
        }

        _previousViewKey = key;
    }
}