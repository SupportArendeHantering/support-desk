using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Tickets.Interfaces;

public interface ITicketRepository
{
    Task AddTicketAsync(SupportTicket ticket);

    Task<IReadOnlyList<SupportTicket>> GetAllTicketsAsync();

    Task UpdateTicketAsync(SupportTicket ticket);
}
