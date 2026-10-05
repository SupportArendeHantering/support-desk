using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SupportDesk.Application;
using SupportDesk.Infrastructure;
using SupportDeskt.Presentation.Navigation;
using System;

namespace SupportDeskt.Presentation;

public partial class App : Microsoft.UI.Xaml.Application
{
    private readonly IServiceProvider _provider;
    private Window? _window;

    public App()
    {
        InitializeComponent();

        ServiceCollection services = new();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddApplication();
        services.AddInfrastructure();

        _provider = services.BuildServiceProvider();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = _provider.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}
