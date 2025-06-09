using KawaiiList.Services;
using KawaiiList.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using KawaiiList.Stores;
using Microsoft.Extensions.Configuration;
using System.IO;
using KawaiiList.Models;

namespace KawaiiList;

public partial class App : Application
{
    private IServiceProvider _serviceProvider;
    private IConfiguration _configuration;

    public App()
    {
        IServiceCollection services = new ServiceCollection();

        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        
        _configuration = builder.Build();
        
        services.Configure<SupabaseSettings>(_configuration.GetSection("Supabase"));

        services.AddSingleton<NavigationStore>();
        services.AddSingleton<AnimeStore>();
        services.AddSingleton<ModalNavigationStore>();
        services.AddSingleton<UserStore>();
        services.AddSingleton<SupabaseClientStore>();

        services.AddHttpClient<IAnilibriaService, AnilibriaService>();
        services.AddHttpClient<IShikimoriService, ShikimoriService>();

        services.AddSingleton<IMediaControlService, MediaControlService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddTransient(typeof(ISupaBaseService<>), typeof(SupaBaseService<>));
        services.AddTransient<IStorageSupabaseService, StorageSupabaseService>();
        services.AddTransient<IScreenService, ScreenService>();
        services.AddTransient<ICursorPositionService, CursorPositionService>();
        services.AddTransient<INavigationService>(s => CreateHomeNavigationService(s));
        services.AddTransient<ICloseModalNavigationService, CloseModalNavigationService>();
        
        services.AddTransient<AnimeCarouselViewModel>(CreateAnimeCarouselViewModel);
        services.AddTransient<ProfileViewModel>(CreateProfileViewModel);
        services.AddTransient<TitleAnimeListViewModel>(CreateTitleAnimeListViewModel);
        services.AddTransient<StatisticsAnimeViewModel>();
        services.AddTransient<AnimeCharactersViewModel>();
        services.AddTransient<AnimeAuthorViewModel>();
        services.AddTransient<RelatedAnimeViewModel>(CreateRelatedAnimeViewModel);
        services.AddTransient<EditingAnimeStatusTitleViewModel>(CreateEditingAnimeStatusTitleViewModel);
        services.AddTransient<EditingProfileViewModel>(CreateEditingProfileViewModel);
        services.AddTransient<SignUpViewModel>(CreateSignUpViewModel);
        services.AddTransient<SignInViewModel>(CreateSignInViewModel);
        services.AddTransient<CatalogViewModel>(CreateCatalogViewModel);
        services.AddTransient<ScheduleViewModel>(CreateScheduleViewModel);
        services.AddTransient<SearchViewModel>(CreateSearchViewModel);
        services.AddTransient<NavigationBarViewModel>(CreateNavigationBarViewModel);
        services.AddTransient<HaderViewModel>(CreateHaderViewModel);
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

    protected override async void OnStartup(StartupEventArgs e)
    {
        _serviceProvider.GetRequiredService<INavigationService>().Navigate();

        await InitializeSupabase();

        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private async Task InitializeSupabase()
    {
        var clientStore = _serviceProvider.GetRequiredService<SupabaseClientStore>();
        await clientStore.InitializeClientAsync();
        await _serviceProvider.GetRequiredService<IAuthService>().TryRestoreSessionAsync();
    }   

    private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider service)
    {
        return new NavigationBarViewModel
        (
            CreateHomeNavigationService(service),
            CreateCatalogNavigationService(service),
            CreateScheduleNavigationService(service),
            CreateAnimeInfoNavigationService(service),
            service.GetRequiredService<AnimeStore>(),
            service.GetRequiredService<IAnilibriaService>(),
            service.GetRequiredService<IShikimoriService>()
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
            service.GetRequiredService<UserStore>(),
            service.GetRequiredService<StatisticsAnimeViewModel>(),
            service.GetRequiredService<RelatedAnimeViewModel>(),
            service.GetRequiredService<AnimeCharactersViewModel>(),
            service.GetRequiredService<AnimeAuthorViewModel>(),
            CreateWatchAnimeNavigationService(service),
            service.GetRequiredService<ISupaBaseService<UserAnimeStatus>>()
        );
    }

    private CatalogViewModel CreateCatalogViewModel(IServiceProvider service)
    {
        return new CatalogViewModel
        (
            service.GetRequiredService<IAnilibriaService>(),
            service.GetRequiredService<IShikimoriService>(),
            service.GetRequiredService<AnimeStore>(),
            CreateAnimeInfoNavigationService(service)
        );
    }

    private SignUpViewModel CreateSignUpViewModel(IServiceProvider service)
    {
        return new SignUpViewModel
        (
            service.GetRequiredService<ICloseModalNavigationService>(),
            CreateSignInNavigationService(service),
            service.GetRequiredService<IAuthService>()
        );
    }

    private SignInViewModel CreateSignInViewModel(IServiceProvider service)
    {
        return new SignInViewModel
        (
            service.GetRequiredService<ICloseModalNavigationService>(),
            CreateSignUpNavigationService(service),
            service.GetRequiredService<IAuthService>()
        );
    }

    private HaderViewModel CreateHaderViewModel(IServiceProvider service)
    {
        return new HaderViewModel
        (
            service.GetRequiredService<SearchViewModel>(),
            service.GetRequiredService<UserStore>(),
            CreateSignInNavigationService(service),
            CreateProfileNavigationService(service),
            service.GetRequiredService<IAuthService>()
        );
    }

    private TitleAnimeListViewModel CreateTitleAnimeListViewModel(IServiceProvider service)
    {
        return new TitleAnimeListViewModel
        (
            service.GetRequiredService<ISupaBaseService<UserAnimeStatus>>(),
            service.GetRequiredService<UserStore>(),
            service.GetRequiredService<AnimeStore>(),
            service.GetRequiredService<IAnilibriaService>(),
            service.GetRequiredService<IShikimoriService>(),
            CreateAnimeInfoNavigationService(service),
            CreateEditingAnimeStatusTitleNavigationService(service)
        );
    }

    private ScheduleViewModel CreateScheduleViewModel(IServiceProvider service)
    {
        return new ScheduleViewModel
        (
            service.GetRequiredService<IAnilibriaService>(),
            service.GetRequiredService<IShikimoriService>(),
            CreateAnimeInfoNavigationService(service),
            service.GetRequiredService<AnimeStore>()
        );
    }

    private ProfileViewModel CreateProfileViewModel(IServiceProvider service)
    {
        return new ProfileViewModel
        (
            service.GetRequiredService<IAnilibriaService>(),
            service.GetRequiredService<UserStore>(),
            CreateHomeNavigationService(service),
            CreateEditingProfileNavigationService(service),
            service.GetRequiredService<TitleAnimeListViewModel>()
        );
    }

    private EditingProfileViewModel CreateEditingProfileViewModel(IServiceProvider service)
    {
        return new EditingProfileViewModel
        (
            service.GetRequiredService<ICloseModalNavigationService>(),
            service.GetRequiredService<ISupaBaseService<Profiles>>(),
            service.GetRequiredService<ISupaBaseService<UserImage>>(),
            service.GetRequiredService<IStorageSupabaseService>(),
            service.GetRequiredService<UserStore>(),
            service.GetRequiredService<SupabaseClientStore>()
        );
    }

    private EditingAnimeStatusTitleViewModel CreateEditingAnimeStatusTitleViewModel(IServiceProvider service)
    {
        return new EditingAnimeStatusTitleViewModel
        (
            service.GetRequiredService<AnimeStore>(),
            service.GetRequiredService<UserStore>(),
            service.GetRequiredService<ICloseModalNavigationService>(),
            service.GetRequiredService<ISupaBaseService<UserAnimeStatus>>()
        );
    }

    private RelatedAnimeViewModel CreateRelatedAnimeViewModel(IServiceProvider service)
    {
        return new RelatedAnimeViewModel
        (
            service.GetRequiredService<IAnilibriaService>(),
            service.GetRequiredService<IShikimoriService>(),
            CreateAnimeInfoNavigationService(service),
            service.GetRequiredService<AnimeStore>()
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

    private INavigationService CreateScheduleNavigationService(IServiceProvider service)
    {
        return new NavigationService<ScheduleViewModel>(service.GetRequiredService<NavigationStore>(),
            () => service.GetRequiredService<ScheduleViewModel>());
    }

    private INavigationService CreateProfileNavigationService(IServiceProvider service)
    {
        return new NavigationService<ProfileViewModel>(service.GetRequiredService<NavigationStore>(),
            () => service.GetRequiredService<ProfileViewModel>());
    }

    private INavigationService CreateSignUpNavigationService(IServiceProvider service)
    {
        return new ModalNavigationService<SignUpViewModel>(service.GetRequiredService<ModalNavigationStore>(),
            () => service.GetRequiredService<SignUpViewModel>());
    }

    private INavigationService CreateSignInNavigationService(IServiceProvider service)
    {
        return new ModalNavigationService<SignInViewModel>(service.GetRequiredService<ModalNavigationStore>(),
            () => service.GetRequiredService<SignInViewModel>());
    }
    
    private INavigationService CreateEditingAnimeStatusTitleNavigationService(IServiceProvider service)
    {
        return new ModalNavigationService<EditingAnimeStatusTitleViewModel>(service.GetRequiredService<ModalNavigationStore>(),
            () => service.GetRequiredService<EditingAnimeStatusTitleViewModel>());
    }

    private INavigationService CreateEditingProfileNavigationService(IServiceProvider service)
    {
        return new ModalNavigationService<EditingProfileViewModel>(service.GetRequiredService<ModalNavigationStore>(),
            () => service.GetRequiredService<EditingProfileViewModel>());
    }
}