using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Tickets.Interfaces;

public interface ITicketRepository
{
    void AddTicket(SupportTicket ticket);

    IReadOnlyList<SupportTicket> GetAllTickets();

    void UpdateTicket(SupportTicket ticket);
}
