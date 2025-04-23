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
        services.AddHttpClient<AnilibriaService>();
        services.AddHttpClient<ShikimoriService>();

        services.AddTransient<AnimeCarouselViewModel>(CreateAnimeCarouselViewModel);
        services.AddTransient<HomeViewModel>();
        services.AddTransient<AnimeInfoViewModel>();
        services.AddTransient<SearchViewModel>(CreateSearchViewModel);

        services.AddSingleton<NavigationBarViewModel>(s => new NavigationBarViewModel
        (
            CreateHomeNavigationService(s)
        ));

        services.AddTransient<HaderViewModel>();

        services.AddSingleton<INavigationService>(s => CreateHomeNavigationService(s));

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

    private SearchViewModel CreateSearchViewModel(IServiceProvider service)
    {
        return new SearchViewModel
        (
            service.GetRequiredService<AnilibriaService>(),
            service.GetRequiredService<AnimeStore>(),
            CreateAnimeInfoNavigationService(service)
        );
    }

    private AnimeCarouselViewModel CreateAnimeCarouselViewModel(IServiceProvider service)
    {
        return new AnimeCarouselViewModel
        (
            service.GetRequiredService<AnilibriaService>(),
            service.GetRequiredService<AnimeStore>(),
            CreateAnimeInfoNavigationService(service)
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
}