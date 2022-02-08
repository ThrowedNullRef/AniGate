using System.Collections.Generic;
using System.Linq;
using AniCore.Core;
using AniCore.Core.AnimeSynchronization;
using AniCore.WpfClient.FrameworkExtensions;

namespace AniCore.WpfClient.Animes;

public sealed class AnimeListViewModel : BaseNotifyPropertyChanged
{
    private readonly AnimeCardActions _actions;
    private List<AnimeCardViewModel> _animeCards = new();

    public AnimeListViewModel(List<Anime> animes, AnimeCardActions actions)
    {
        _actions = actions;
        SetAnimes(animes);
    }

    public List<AnimeCardViewModel> AnimeCardViewModels
    {
        get => _animeCards;
        set => SetIfDifferent(ref _animeCards, value);
    }

    public void SetAnimes(List<Anime> animes)
    {
        AnimeCardViewModels = animes.Select(a => new AnimeCardViewModel(a, _actions)).ToList();
    }
}