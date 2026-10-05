using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SupportDeskt.Presentation.Navigation;

namespace SupportDeskt.Presentation;

public sealed partial class MainWindow : Window
{
    public INavigationService NavigationService { get; }

    public MainWindow(INavigationService navigationService)
    {
        InitializeComponent();

        NavigationService = navigationService;

        NavigationService.Navigate(AppPage.Home);
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