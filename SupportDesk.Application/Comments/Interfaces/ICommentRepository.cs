using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Comments.Interfaces;

public interface ICommentRepository
{
    Task AddCommentAsync(TicketComment ticketComment);
    Task<IReadOnlyList<TicketComment>> GetCommentsByTicketIdAsync(Guid ticketId);

}
