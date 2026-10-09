using CommunityToolkit.Mvvm.ComponentModel;
using SupportDesk.Application.Tickets;
using System.Threading.Tasks;
using SupportDesk.Domain.Models;
using System.Linq;

namespace SupportDeskt.Presentation.ViewModels;

public partial class DashboardPageViewModel(ITicketService ticketService) : ObservableObject
{
    [ObservableProperty]
    public partial int TotalTickets { get; private set; }

    [ObservableProperty]
    public partial int NewTickets { get; private set; }

    [ObservableProperty]
    public partial int OngoingTickets { get; private set; }

    [ObservableProperty]
    public partial int ResolvedTickets { get; private set; }

    [ObservableProperty]
    public partial double NewTicketsPercentage { get; private set; }

    [ObservableProperty]
    public partial double OngoingTicketsPercentage { get; private set; }

    [ObservableProperty]
    public partial double ResolvedTicketsPercentage { get; private set; }



    public async Task LoadTicketsAsync()
    {
        var tickets = await ticketService.GetAllTicketsAsync();
        TotalTickets = tickets.Count;

        NewTickets = tickets.Count(ticket => ticket.Status == TicketStatus.New);
        OngoingTickets = tickets.Count(ticket => ticket.Status == TicketStatus.Ongoing);
        ResolvedTickets = tickets.Count(ticket => ticket.Status == TicketStatus.Resolved);

        if (TotalTickets > 0)
        {
            NewTicketsPercentage = (double)NewTickets / TotalTickets * 100;
            OngoingTicketsPercentage = (double)OngoingTickets / TotalTickets * 100;
            ResolvedTicketsPercentage = (double)ResolvedTickets / TotalTickets * 100;
        }
        else
        {
            NewTicketsPercentage = 0;
            OngoingTicketsPercentage = 0;
            ResolvedTicketsPercentage = 0;
            
        }


    }
}
