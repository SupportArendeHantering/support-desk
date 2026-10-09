using CommunityToolkit.Mvvm.ComponentModel;
using SupportDesk.Application.Tickets;
using System.Threading.Tasks;
using SupportDesk.Domain.Models;
using System.Linq;
using System;

namespace SupportDeskt.Presentation.ViewModels;

public partial class DashboardPageViewModel(ITicketService ticketService) : ObservableObject
{
    [ObservableProperty]
    public partial int TotalTickets { get; private set; }

    [ObservableProperty]
    public partial double NewTickets { get; private set; }

    [ObservableProperty]
    public partial double OngoingTickets { get; private set; }

    [ObservableProperty]
    public partial double ResolvedTickets { get; private set; }

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
            NewTicketsPercentage = Math.Round(NewTickets / TotalTickets * 100);
            OngoingTicketsPercentage =  Math.Round(OngoingTickets / TotalTickets * 100);
            ResolvedTicketsPercentage = Math.Round(ResolvedTickets / TotalTickets * 100);
        }
        else
        {
            NewTicketsPercentage = 0;
            OngoingTicketsPercentage = 0;
            ResolvedTicketsPercentage = 0;
            
        }


    }
}
