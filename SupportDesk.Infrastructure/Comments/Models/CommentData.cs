namespace SupportDesk.Infrastructure.Comments.Models;

public class CommentData
{
    public Guid CommentId { get; set; }
    public Guid TicketId { get; set; }
    public Guid CreatedBy { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public DateTime CreatedAt {  get; set; }

}
