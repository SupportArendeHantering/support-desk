using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Domain.Models;

public class TicketComment
{
    public Guid CommentId { get; set; }
    public Guid TicketId { get; set; }
    public int CreatedBy { get; set; }
    public required CommentText CommentText { get; set; }
    public DateTime CreatedAt { get; set; }
}