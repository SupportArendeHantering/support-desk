using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SupportDesk.Domain.Models;
using SupportDeskt.Presentation.ViewModels;

namespace SupportDeskt.Presentation.Views;

public sealed partial class TicketsPage : Page
{
    public TicketsPageViewModel ViewModel { get; }

    public TicketsPage(
        TicketsPageViewModel viewModel)
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

    private void TicketsList_ItemClick(
        object sender,
        ItemClickEventArgs e)
    {
        if (e.ClickedItem is SupportTicket ticket)
        {
            ViewModel.OpenTicket(ticket);
        }
    }
}