using SupportDesk.Domain.Models;
using SupportDesk.Infrastructure.Comments.Models;

namespace SupportDesk.Infrastructure.Tickets.Models;

public class TicketData
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? AssigneeId { get; set; }
    public Guid AssignedCustomerId { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CommentData> Comments { get; set; } = [];
}
