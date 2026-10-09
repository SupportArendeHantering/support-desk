using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupportDesk.Application.Customers;
using SupportDesk.Application.Tickets;
using SupportDesk.Domain.Models;
using SupportDeskt.Presentation.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SupportDeskt.Presentation.ViewModels;

public partial class TicketsPageViewModel(
    ITicketService ticketService,
    ICustomerService customerService,
    INavigationService navigationService) : ObservableObject
{
    public ObservableCollection<SupportTicket> Tickets { get; } = [];

    public ObservableCollection<Customer> Customers { get; } = [];

    public ObservableCollection<TicketStatus> Statuses { get; } =
        new(Enum.GetValues<TicketStatus>());

    public ObservableCollection<TicketPriority> Priorities { get; } =
        new(Enum.GetValues<TicketPriority>());

    [ObservableProperty]
    public partial string SearchTerm { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Customer? SelectedCustomer { get; set; }

    [ObservableProperty]
    public partial TicketStatus? SelectedStatus { get; set; }

    [ObservableProperty]
    public partial string NewTicketTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewTicketDescription { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Customer? NewTicketCustomer { get; set; }

    [ObservableProperty]
    public partial TicketPriority NewTicketPriority { get; set; } = TicketPriority.Low;

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
            var customers = await customerService.GetAllCustomersAsync();

            UpdateTicketList(tickets);

            Customers.Clear();

            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }

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
    private async Task CreateTicketAsync()
    {
        if (NewTicketCustomer is null)
        {
            StatusMessage = "Välj en kund.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var newTicket = await ticketService.RegisterTicketAsync(
                NewTicketTitle,
                NewTicketDescription,
                NewTicketCustomer.Id,
                NewTicketPriority);

            Tickets.Insert(0, newTicket);

            NewTicketTitle = string.Empty;
            NewTicketDescription = string.Empty;
            NewTicketCustomer = null;
            NewTicketPriority = TicketPriority.Low;

            StatusMessage = "Ärendet skapades.";
        }
        catch (ArgumentException ex)
        {
            StatusMessage = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }
        catch (Exception)
        {
            StatusMessage = "Ärendet kunde inte skapas.";
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
            var tickets = await ticketService.GetAllTicketsAsync();

            IEnumerable<SupportTicket> filteredTickets = tickets;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                filteredTickets = filteredTickets.Where(ticket =>
                    ticket.Title.Value.Contains(
                        SearchTerm,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedCustomer is not null)
            {
                filteredTickets = filteredTickets.Where(ticket =>
                    ticket.AssignedCustomerId == SelectedCustomer.Id);
            }

            if (SelectedStatus is not null)
            {
                filteredTickets = filteredTickets.Where(ticket =>
                    ticket.Status == SelectedStatus.Value);
            }

            UpdateTicketList(
                filteredTickets
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .ToList());

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
        SelectedCustomer = null;
        SelectedStatus = null;

        await LoadAsync();
    }

    public void OpenTicket(SupportTicket ticket)
    {
        navigationService.NavigateToTicketDetails(ticket);
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