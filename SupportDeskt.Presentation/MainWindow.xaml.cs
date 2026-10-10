using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SupportDesk.Application.Notifications;
using SupportDeskt.Presentation.Navigation;

namespace SupportDeskt.Presentation;

public sealed partial class MainWindow : Window
{
    public INavigationService NavigationService { get; }
    private readonly INotificationService _notificationService;

    public MainWindow(INavigationService navigationService, INotificationService notificationService)
    {
        InitializeComponent();

        _notificationService = notificationService;

        SetupInfoBarEvents();

        NavigationService = navigationService;

        NavigationService.Navigate(AppPage.Home);
    }

    private void SetupInfoBarEvents()
    {
        _notificationService.ShowInfoBarRequested += (sender, args) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                StatusInfoBar.Message = args.Message;
                StatusInfoBar.Severity = args.Type switch
                {
                    InfoType.Success => InfoBarSeverity.Success,
                    InfoType.Warning => InfoBarSeverity.Warning,
                    InfoType.Error => InfoBarSeverity.Error,
                    _ => InfoBarSeverity.Informational
                };
                StatusInfoBar.IsOpen = true;
            });
        };
    }

    private void MainNavigation_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer is not NavigationViewItem selectedItem)
            return;

        switch (selectedItem.Tag?.ToString())
        {
            case "Home":
                NavigationService.Navigate(AppPage.Home);
                break;

            case "Ticket":
                NavigationService.Navigate(AppPage.Ticket);
                break;

            case "Customer":
                NavigationService.Navigate(AppPage.Customer);
                break;
        }
    }
}