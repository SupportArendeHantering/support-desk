using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Tickets.Interfaces;

public interface ITicketRepository
{
    void AddTicket(SupportTicket ticket);
}
