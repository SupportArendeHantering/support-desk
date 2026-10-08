using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Comments
{
    public interface ICommentService
    {
        Task AddCommentAsync(Guid ticketId, int assigneeId, string commentText);

        Task<IReadOnlyList<TicketComment>> GetCommentsByTicketIdAsync(Guid ticketId);
    }
}