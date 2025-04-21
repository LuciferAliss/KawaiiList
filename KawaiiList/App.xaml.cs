using KawaiiList.Services;
using KawaiiList.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using KawaiiList.Views;
using KawaiiList.Stores;
using KawaiiList.Components;
using KawaiiList.Models.Anilibria;

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

        services.AddTransient<AnimeCarouselViewModel>(s => new AnimeCarouselViewModel(s.GetRequiredService<AnilibriaService>()));
        services.AddTransient<HomeViewModel>(s => new HomeViewModel(s.GetRequiredService<AnimeCarouselViewModel>()));
        services.AddTransient<AnimeInfoViewModel>(s => new AnimeInfoViewModel(s.GetRequiredService<AnimeStore>()));

        services.AddSingleton<NavigationBarViewModel>(s => new NavigationBarViewModel
        (
            CreateHomeNavigationService(s)
        ));

        services.AddTransient<SearchViewModel>(s => new SearchViewModel
        (
            s.GetRequiredService<AnilibriaService>(),
            s.GetRequiredService<AnimeStore>(),
            CreateAnimeInfoNavigationService(s)
        ));

        services.AddTransient<HaderViewModel>(s => new HaderViewModel(s.GetRequiredService<SearchViewModel>()));

        services.AddSingleton<INavigationService>(s => CreateHomeNavigationService(s));

        services.AddSingleton<MainViewModel>(s => new MainViewModel
        (
            s.GetRequiredService<NavigationStore>(),
            s.GetRequiredService<NavigationBarViewModel>(),
            s.GetRequiredService<HaderViewModel>()
        ));

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