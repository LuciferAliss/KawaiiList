using KawaiiList.Services;
using KawaiiList.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using KawaiiList.Stores;

namespace KawaiiList;

public partial class App : Application
{
    private IServiceProvider _serviceProvider;

    public App()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<NavigationStore>();
        services.AddSingleton<AnimeStore>();

        services.AddHttpClient<IAnilibriaService, AnilibriaService>();
        services.AddHttpClient<IShikimoriService, ShikimoriService>();

        services.AddSingleton<IMediaControlService, MediaControlService>();
        services.AddTransient<IScreenService, ScreenService>();
        services.AddTransient<ICursorPositionService, CursorPositionService>();
        services.AddTransient<INavigationService>(s => CreateHomeNavigationService(s));

        services.AddTransient<AnimeCarouselViewModel>(CreateAnimeCarouselViewModel);
        services.AddTransient<StatisticsAnimeViewModel>();
        services.AddTransient<CatalogViewModel>();
        services.AddTransient<SearchViewModel>(CreateSearchViewModel);
        services.AddSingleton<NavigationBarViewModel>(CreateNavigationBarViewModel);
        services.AddTransient<HaderViewModel>();

        services.AddTransient<HomeViewModel>();
        services.AddTransient<AnimeInfoViewModel>(CreateWatchAnimeViewModel);
        services.AddTransient<WatchAnimeViewModel>();

        services.AddSingleton<MainViewModel>();

        services.AddSingleton<MainWindow>(s => new MainWindow()
        {
            DataContext = s.GetRequiredService<MainViewModel>()
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _serviceProvider.GetRequiredService<INavigationService>().Navigate();

        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider service)
    {
        return new NavigationBarViewModel
        (
            CreateHomeNavigationService(service),
            CreateCatalogNavigationService(service)
        );
    }

    private SearchViewModel CreateSearchViewModel(IServiceProvider service)
    {
        return new SearchViewModel
        (
            service.GetRequiredService<IAnilibriaService>(),
            service.GetRequiredService<IShikimoriService>(),
            service.GetRequiredService<AnimeStore>(),
            CreateAnimeInfoNavigationService(service)
        );
    }

    private AnimeCarouselViewModel CreateAnimeCarouselViewModel(IServiceProvider service)
    {
        return new AnimeCarouselViewModel
        (
            service.GetRequiredService<IAnilibriaService>(),
            service.GetRequiredService<IShikimoriService>(),
            service.GetRequiredService<AnimeStore>(),
            CreateAnimeInfoNavigationService(service)
        );
    }

    private AnimeInfoViewModel CreateWatchAnimeViewModel (IServiceProvider service)
    {
        return new AnimeInfoViewModel
        (
            service.GetRequiredService<AnimeStore>(),
            service.GetRequiredService<StatisticsAnimeViewModel>(),
            CreateWatchAnimeNavigationService(service)
        );
    }

    private INavigationService CreateAnimeInfoNavigationService(IServiceProvider service)
    {
        return new NavigationService<AnimeInfoViewModel>(service.GetRequiredService<NavigationStore>(),
            () => service.GetRequiredService<AnimeInfoViewModel>());
    }

    private INavigationService CreateHomeNavigationService(IServiceProvider service)
    {
        return new NavigationService<HomeViewModel>(service.GetRequiredService<NavigationStore>(),
            () => service.GetRequiredService<HomeViewModel>());
    }

    private INavigationService CreateWatchAnimeNavigationService(IServiceProvider service)
    {
        return new NavigationService<WatchAnimeViewModel>(service.GetRequiredService<NavigationStore>(),
            () => service.GetRequiredService<WatchAnimeViewModel>());
    }

    private INavigationService CreateCatalogNavigationService(IServiceProvider service)
    {
        return new NavigationService<CatalogViewModel>(service.GetRequiredService<NavigationStore>(),
            () => service.GetRequiredService<CatalogViewModel>());
    }
}