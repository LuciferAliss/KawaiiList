using KawaiiList.Service.API;
using KawaiiList.Services.API;
using KawaiiList.View;
using KawaiiList.ViewModels.MainVm;
using KawaiiList.ViewModels.LoginVm;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using KawaiiList.ViewModels.AnimeCarouselVM;
using System.Windows.Controls;
using KawaiiList.Views.Controls;

namespace KawaiiList;

public partial class App : Application
{
    private IServiceProvider ServiceProvider { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);

        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = ServiceProvider.GetRequiredService<MainPage>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database
        //services.AddDbContext<AppDbContext>();

        // HTTP Client
        services.AddHttpClient<IApiService, AnilibriaService>();

        // Services
        //services.AddSingleton<ISecurityService, SecurityService>();
        //services.AddScoped<IUserRepository, UserRepository>();
        //services.AddScoped<IAuthService, AuthService>();

        // ViewModels
        services.AddTransient<IAnimeCarouselViewModel, AnimeCarouselViewModel>();
        services.AddTransient<IMainViewModel, MainViewModel>();
        services.AddTransient<LoginViewModel>();

        //Control
        services.AddTransient<AnimeCarouselControl>();

        // Windows
        services.AddSingleton<MainPage>();
        services.AddTransient<LoginPage>();
    }
}
