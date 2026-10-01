using SupportDesk.Application.Comments.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Application.Comments;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }
    public async Task AddCommentAsync(
        Guid ticketId,
        Guid assigneeId,
        string commentText)
    {
        if (ticketId == Guid.Empty)
            throw new ArgumentException("Ticket is required.", nameof(ticketId));
        if (assigneeId == Guid.Empty)
            throw new ArgumentException("Assignee is required.", nameof(assigneeId));

        CommentText validatedCommentText = new(commentText);

        TicketComment ticketComment = new()
        {
            CommentId = Guid.NewGuid(),
            TicketId = ticketId,
            CreatedBy = assigneeId,
            CommentText = validatedCommentText,
            CreatedAt = DateTime.UtcNow
        };
        await _commentRepository.AddCommentAsync(ticketComment);
    }

    public async Task<IReadOnlyList<TicketComment>> GetCommentsByTicketIdAsync(Guid ticketId)
    {
        if (ticketId == Guid.Empty)
            throw new ArgumentException("Ticket is required.", nameof(ticketId));

        return await _commentRepository.GetCommentsByTicketIdAsync(ticketId);
    }
}
