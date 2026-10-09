using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using SupportDesk.Domain.Models;
using SupportDeskt.Presentation.Views;
using System;

namespace SupportDeskt.Presentation.Navigation;

public partial class NavigationService(
    IServiceProvider services) : ObservableObject, INavigationService
{
    [ObservableProperty]
    public partial Page? CurrentPage { get; private set; }

    public SupportTicket? SelectedTicket { get; private set; }

    public void Navigate(AppPage page)
    {
        CurrentPage = page switch
        {
            AppPage.Home =>
                services.GetRequiredService<DashboardPage>(),

            AppPage.Customer =>
                services.GetRequiredService<CustomersPage>(),

            AppPage.Ticket =>
                services.GetRequiredService<TicketsPage>(),

            AppPage.TicketDetails =>
                services.GetRequiredService<TicketDetailsPage>(),

            _ => throw new ArgumentOutOfRangeException(nameof(page))
        };
    }

    public void NavigateToTicketDetails(SupportTicket ticket)
    {
        SelectedTicket = ticket;

        Navigate(AppPage.TicketDetails);
    }

    public void BackToTickets()
    {
        Navigate(AppPage.Ticket);
    }
}