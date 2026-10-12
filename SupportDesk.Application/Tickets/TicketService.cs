using SupportDesk.Application.Customers;
using SupportDesk.Application.Tickets.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Application.Tickets;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ICustomerService _customerService;

    public TicketService(ITicketRepository ticketRepository, ICustomerService customerService)
    {
        _ticketRepository = ticketRepository;
        _customerService = customerService;
    }

    public async Task<SupportTicket> RegisterTicketAsync(
     string title,
     string description,
     Guid customerId,
     TicketPriority priority)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "Customer is required.",
                nameof(customerId));

        TicketTitle ticketTitle = new(title);
        TicketDescription ticketDescription = new(description);

        SupportTicket supportTicket = new(ticketTitle, ticketDescription)
        {
            Id = Guid.NewGuid(),
            AssignedCustomerId = customerId,
            Priority = priority
        };

        await _ticketRepository.AddTicketAsync(supportTicket);

        return supportTicket;
    }

    public async Task<IReadOnlyList<SupportTicket>> GetAllTicketsAsync()
    {
        var tickets = await _ticketRepository.GetAllTicketsAsync();

        return tickets
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }

    public async Task UpdateTicketDetailsAsync(
     SupportTicket ticket,
     string title,
     string description)
    {
        ticket.ChangeTitle(title);
        ticket.ChangeDescription(description);

        await _ticketRepository.UpdateTicketAsync(ticket);
    }

    public async Task ChangePriorityAsync(
    SupportTicket ticket,
    TicketPriority priority)
    {
        ticket.ChangePriority(priority);

        await _ticketRepository.UpdateTicketAsync(ticket);
    }

    public async Task ChangeStatusAsync(
    SupportTicket ticket,
    TicketStatus status)
    {
        ticket.ChangeStatus(status);

        await _ticketRepository.UpdateTicketAsync(ticket);
    }

    public async Task AssignCustomerToTicketAsync(
        SupportTicket ticket,
        Guid customerId)
    {
        ticket.AssignCustomerId(customerId);
        await _ticketRepository.UpdateTicketAsync(ticket);
    }

    public async Task<Customer> GetAssignedCustomerAsync(SupportTicket ticket)
    {
        var assignedCustomer = await _customerService.GetCustomerByIdAsync(ticket.AssignedCustomerId);
        return assignedCustomer;
    }

    public async Task AssignAssigneeAsync(SupportTicket ticket, int assigneeId)
    {
        ticket.AssignAssignee(assigneeId);
        await _ticketRepository.UpdateTicketAsync(ticket);
    }

    public async Task<IReadOnlyList<SupportTicket>> SearchTicketsByTitleAsync(string searchTerm)
    {
        var tickets = await _ticketRepository.GetAllTicketsAsync();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return tickets
                .OrderByDescending(ticket => ticket.CreatedAt)
                .ToList();
        }

        return tickets
            .Where(ticket =>
                ticket.Title.Value.Contains(
                    searchTerm,
                    StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<SupportTicket>> SearchTicketsByCustomerNameAsync(string searchTerm)
    {
        var tickets = await _ticketRepository.GetAllTicketsAsync();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return tickets
                .OrderByDescending(ticket => ticket.CreatedAt)
                .ToList();
        }

        var customers = await _customerService.GetAllCustomersAsync();

        var matchingCustomerIds = customers
            .Where(customer =>
                customer.Name.Value.Contains(
                    searchTerm,
                    StringComparison.OrdinalIgnoreCase))
            .Select(customer => customer.Id)
            .ToHashSet();

        return tickets
            .Where(ticket =>
                matchingCustomerIds.Contains(ticket.AssignedCustomerId))
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<SupportTicket>> GetTicketsByStatusAsync(IReadOnlyList<TicketStatus>? statuses)
    {
        var allTickets = await _ticketRepository.GetAllTicketsAsync();

        if (statuses == null || !statuses.Any())
        {
            return allTickets;
        }

        return [.. allTickets.Where(ticket => statuses.Contains(ticket.Status))];
    }

    public async Task<IReadOnlyList<SupportTicket>> SearchAndFilterTicketsAsync(
    string? searchTerm,
    IReadOnlyList<TicketStatus>? statuses)
    {
        var tickets = await _ticketRepository.GetAllTicketsAsync();

        if (statuses != null && statuses.Any())
        {
            tickets = tickets
                .Where(ticket => statuses.Contains(ticket.Status))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var customers = await _customerService.GetAllCustomersAsync();

            var matchingCustomerIds = customers
                .Where(customer =>
                    customer.Name.Value.Contains(
                        searchTerm,
                        StringComparison.OrdinalIgnoreCase))
                .Select(customer => customer.Id)
                .ToHashSet();

            tickets = tickets
                .Where(ticket =>
                    ticket.Title.Value.Contains(
                        searchTerm,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    matchingCustomerIds.Contains(ticket.AssignedCustomerId))
                .ToList();
        }

        return tickets
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ToList();
    }
}
