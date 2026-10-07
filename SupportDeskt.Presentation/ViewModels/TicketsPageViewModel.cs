using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupportDesk.Application.Tickets;
using SupportDesk.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SupportDeskt.Presentation.ViewModels;

public partial class TicketsPageViewModel(ITicketService ticketService) : ObservableObject
{
    public ObservableCollection<SupportTicket> Tickets { get; } = [];

    public ObservableCollection<TicketStatus> Statuses { get; } =
        new(Enum.GetValues<TicketStatus>());

    [ObservableProperty]
    public partial string SearchTerm { get; set; } = string.Empty;

    [ObservableProperty]
    public partial TicketStatus? SelectedStatus { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; private set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; private set; }

    public bool IsNotBusy => !IsBusy;

    public async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var tickets = await ticketService.GetAllTicketsAsync();

            UpdateTicketList(tickets);

            StatusMessage = $"{Tickets.Count} ärenden.";
        }
        catch (Exception)
        {
            StatusMessage = "Ärendena kunde inte laddas.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            IReadOnlyList<TicketStatus>? statuses = null;

            if (SelectedStatus is not null)
            {
                statuses = [SelectedStatus.Value];
            }

            var tickets = await ticketService.SearchAndFilterTicketsAsync(
                SearchTerm,
                statuses);

            UpdateTicketList(tickets);

            StatusMessage = $"{Tickets.Count} ärenden hittades.";
        }
        catch (Exception)
        {
            StatusMessage = "Sökningen kunde inte genomföras.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        SearchTerm = string.Empty;
        SelectedStatus = null;

        await LoadAsync();
    }

    private void UpdateTicketList(IEnumerable<SupportTicket> tickets)
    {
        Tickets.Clear();

        foreach (var ticket in tickets)
        {
            Tickets.Add(ticket);
        }
    }
}