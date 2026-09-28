using SupportDesk.Application.Tickets.Interfaces;
using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Tickets;

public class TicketService
{
    private readonly ITicketRepository _ticketRepository;

    public TicketService(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public SupportTicket RegisterTicket(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");

        SupportTicket supportTicket = new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description
        };

        _ticketRepository.AddTicket(supportTicket);

        return supportTicket;

    }
}
