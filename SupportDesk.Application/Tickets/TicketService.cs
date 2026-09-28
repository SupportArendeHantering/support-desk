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

    public SupportTicket RegisterTicket(string title, string description)
    {
        TicketTitle ticketTitle = new(title);
        TicketDescription ticketDescription = new(description);


        SupportTicket supportTicket = new(ticketTitle, ticketDescription)
        {
            Id = Guid.NewGuid(),
        };

        _ticketRepository.AddTicket(supportTicket);

        return supportTicket;

    }
}
