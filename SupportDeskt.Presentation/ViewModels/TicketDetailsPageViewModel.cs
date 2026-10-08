using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupportDesk.Application.Customers;
using SupportDesk.Application.Tickets;
using SupportDesk.Domain.Models;
using SupportDeskt.Presentation.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SupportDeskt.Presentation.ViewModels;

public partial class TicketDetailsPageViewModel(
    INavigationService navigationService,
    ICustomerService customerService,
    ITicketService ticketService,
    ITicketAssigneeService assigneeService) : ObservableObject
{
    [ObservableProperty]
    public partial SupportTicket? Ticket { get; private set; }

    public ObservableCollection<Customer> Customers { get; } = [];

    public ObservableCollection<TicketAssignee> Assignees { get; } = [];

    public ObservableCollection<TicketPriority> Priorities { get; } =
        new(Enum.GetValues<TicketPriority>());

    public ObservableCollection<TicketStatus> Statuses { get; } =
        new(Enum.GetValues<TicketStatus>());

    [ObservableProperty]
    public partial string EditTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditDescription { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Customer? SelectedCustomer { get; set; }

    [ObservableProperty]
    public partial TicketAssignee? SelectedAssignee { get; set; }

    [ObservableProperty]
    public partial TicketPriority SelectedPriority { get; set; }

    [ObservableProperty]
    public partial TicketStatus SelectedStatus { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; private set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; private set; }

    public bool IsNotBusy => !IsBusy;

    public async Task LoadAsync()
    {
        Ticket = navigationService.SelectedTicket;

        if (Ticket is null)
        {
            StatusMessage = "Inget ärende har valts.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            EditTitle = Ticket.Title.Value;
            EditDescription = Ticket.Description.Value;
            SelectedPriority = Ticket.Priority;
            SelectedStatus = Ticket.Status;

            var customers = await customerService.GetAllCustomersAsync();

            Customers.Clear();

            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }

            SelectedCustomer = null;

            foreach (var customer in Customers)
            {
                if (customer.Id == Ticket.AssignedCustomerId)
                {
                    SelectedCustomer = customer;
                    break;
                }
            }

            Assignees.Clear();

            foreach (var assignee in assigneeService.GetAllAssignees())
            {
                Assignees.Add(assignee);
            }

            SelectedAssignee = null;

            if (Ticket.AssigneeId is int assigneeId)
            {
                SelectedAssignee =
                    assigneeService.GetAssigneeById(assigneeId);
            }
        }
        catch (Exception)
        {
            StatusMessage =
                "Alla uppgifter för ärendet kunde inte laddas.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Ticket is null)
        {
            StatusMessage = "Inget ärende har valts.";
            return;
        }

        if (SelectedCustomer is null)
        {
            StatusMessage = "Välj en kund.";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            await ticketService.UpdateTicketDetailsAsync(
                Ticket,
                EditTitle,
                EditDescription);

            if (Ticket.AssignedCustomerId != SelectedCustomer.Id)
            {
                await ticketService.AssignCustomerToTicketAsync(
                    Ticket,
                    SelectedCustomer.Id);
            }

            if (Ticket.Priority != SelectedPriority)
            {
                await ticketService.ChangePriorityAsync(
                    Ticket,
                    SelectedPriority);
            }

            if (SelectedAssignee is not null &&
                Ticket.AssigneeId != SelectedAssignee.Id)
            {
                await ticketService.AssignAssigneeAsync(
                    Ticket,
                    SelectedAssignee.Id);
            }

            if (Ticket.Status != SelectedStatus)
            {
                await ticketService.ChangeStatusAsync(
                    Ticket,
                    SelectedStatus);
            }

            StatusMessage = "Ärendet uppdaterades.";
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
            StatusMessage = "Ärendet kunde inte uppdateras.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Back()
    {
        navigationService.BackToTickets();
    }
}