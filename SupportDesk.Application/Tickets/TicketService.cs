using SupportDesk.Application.Tickets.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Application.Tickets;

public class TicketService
{
    private readonly ITicketRepository _ticketRepository;

    public TicketService(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
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
}
