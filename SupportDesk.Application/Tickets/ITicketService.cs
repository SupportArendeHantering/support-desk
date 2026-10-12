using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Tickets
{
    public interface ITicketService
    {
        Task AssignCustomerToTicketAsync(SupportTicket ticket, Guid customerId);
        Task ChangePriorityAsync(SupportTicket ticket, TicketPriority priority);
        Task ChangeStatusAsync(SupportTicket ticket, TicketStatus status);
        Task<IReadOnlyList<SupportTicket>> GetAllTicketsAsync();
        Task<Customer> GetAssignedCustomerAsync(SupportTicket ticket);
        Task<SupportTicket> RegisterTicketAsync(string title, string description, Guid customerId, TicketPriority priority);
        Task UpdateTicketDetailsAsync(SupportTicket ticket, string title, string description);
        Task AssignAssigneeAsync(SupportTicket ticket, int assigneeId);
        Task<IReadOnlyList<SupportTicket>> SearchTicketsByTitleAsync(string searchTerm);
        Task<IReadOnlyList<SupportTicket>> SearchTicketsByCustomerNameAsync(string searchTerm);
        Task<IReadOnlyList<SupportTicket>> GetTicketsByStatusAsync(IReadOnlyList<TicketStatus>? statuses);
        Task<IReadOnlyList<SupportTicket>> SearchAndFilterTicketsAsync(string? searchTerm,IReadOnlyList<TicketStatus>? statuses);
    }
}