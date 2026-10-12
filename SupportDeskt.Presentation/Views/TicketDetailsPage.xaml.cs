using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SupportDeskt.Presentation.ViewModels;

namespace SupportDeskt.Presentation.Views;

public sealed partial class TicketDetailsPage : Page
{
    public TicketDetailsPageViewModel ViewModel { get; }

    public TicketDetailsPage(
        TicketDetailsPageViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();
    }

    private async void Page_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await ViewModel.LoadAsync();
    }
}