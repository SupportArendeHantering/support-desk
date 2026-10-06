using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SupportDesk.Application;
using SupportDesk.Infrastructure;
using SupportDeskt.Presentation.Navigation;
using SupportDeskt.Presentation.ViewModels;
using SupportDeskt.Presentation.Views;
using System;

namespace SupportDeskt.Presentation;

public partial class App : Application
{
    private readonly IServiceProvider _provider;
    private Window? _window;

    public App()
    {
        InitializeComponent();

        ServiceCollection services = new();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddTransient<DashboardPage>();
        services.AddTransient<CustomersPage>();
        services.AddTransient<TicketsPage>();

        services.AddTransient<CustomersPageViewModel>();

        services.AddApplication();
        services.AddInfrastructure();

        _provider = services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = _provider.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}