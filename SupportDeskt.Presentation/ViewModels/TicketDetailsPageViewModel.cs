using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupportDesk.Application.Customers;
using SupportDesk.Application.Tickets;
using SupportDesk.Domain.Models;
using SupportDeskt.Presentation.Navigation;
using System;
using System.Threading.Tasks;

namespace SupportDeskt.Presentation.ViewModels;

public partial class TicketDetailsPageViewModel(
    INavigationService navigationService,
    ICustomerService customerService,
    ITicketAssigneeService assigneeService) : ObservableObject
{
    [ObservableProperty]
    public partial SupportTicket? Ticket { get; private set; }

    [ObservableProperty]
    public partial string CustomerName { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string AssigneeName { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string StatusMessage { get; private set; } = string.Empty;

    public async Task LoadAsync()
    {
        Ticket = navigationService.SelectedTicket;

        if (Ticket is null)
        {
            StatusMessage = "Inget ärende har valts.";
            return;
        }

        try
        {
            var customer =
                await customerService.GetCustomerByIdAsync(
                    Ticket.AssignedCustomerId);

            CustomerName = customer.Name.Value;

            if (Ticket.AssigneeId is int assigneeId)
            {
                var assignee =
                    assigneeService.GetAssigneeById(assigneeId);

                AssigneeName =
                    assignee?.Name ?? "Okänd handläggare";
            }
            else
            {
                AssigneeName = "Ingen handläggare";
            }
        }
        catch (Exception)
        {
            StatusMessage =
                "Alla uppgifter för ärendet kunde inte laddas.";
        }
    }

    [RelayCommand]
    private void Back()
    {
        navigationService.BackToTickets();
    }
}