using CommunityToolkit.Mvvm.ComponentModel;
using SupportDesk.Application.Tickets;
using System.Threading.Tasks;
using SupportDesk.Domain.Models;
using System.Linq;
using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

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

    [ObservableProperty]
    public partial ISeries[] Series { get; private set; } = [];

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
            OngoingTicketsPercentage = Math.Round(OngoingTickets / TotalTickets * 100);
            ResolvedTicketsPercentage = Math.Round(ResolvedTickets / TotalTickets * 100);
        }
        else
        {
            NewTicketsPercentage = 0;
            OngoingTicketsPercentage = 0;
            ResolvedTicketsPercentage = 0;

        }


        Series =
        [
            new PieSeries<double>
            {
                Name = "Nya",
                Values = [NewTickets],
                InnerRadius = 50,
                Fill = new SolidColorPaint(SKColor.Parse("#E57373")),
                ToolTipLabelFormatter = point => $"{NewTicketsPercentage}%"
            },
            new PieSeries<double>
            {
                Name = "Pågående",
                Values = [OngoingTickets],
                InnerRadius = 50,
                Fill = new SolidColorPaint(SKColor.Parse("#E6C65C")),
                ToolTipLabelFormatter = point => $"{OngoingTicketsPercentage}%"
            },
            new PieSeries<double>
            {
                Name = "Lösta",
                Values = [ResolvedTickets],
                InnerRadius = 50,
                Fill = new SolidColorPaint(SKColor.Parse("#66BB6A")),
                ToolTipLabelFormatter = point => $"{ResolvedTicketsPercentage}%"
            }
        ];


    }
}