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

    public SupportTicket RegisterTicket(
        string title,
        string description,
        Guid customerId,
        TicketPriority priority)

    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer is required.", nameof(customerId));

        TicketTitle ticketTitle = new(title);
        TicketDescription ticketDescription = new(description);


        SupportTicket supportTicket = new(ticketTitle, ticketDescription)
        {
            Id = Guid.NewGuid(),
            AssignedCustomerId = customerId,
            Priority = priority
        };

        _ticketRepository.AddTicket(supportTicket);

        return supportTicket;

    }
    public IReadOnlyList<SupportTicket> GetAllTickets()
    {
        var tickets = _ticketRepository
            .GetAllTickets()
            .OrderByDescending(t => t.CreatedAt)
            .ToList();


        return tickets;
    }
}
