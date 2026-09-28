using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Domain.Models;

public class SupportTicket(TicketTitle title, TicketDescription description)
{
    public Guid Id { get; set; }
    public TicketTitle Title { get; set; } = title;
    public TicketDescription Description { get; set; } = description;
    public Guid? Assignee { get; set; }
    public Guid AssignedCustomerId { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Low;
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void ChangeTitle(string title)
    {
        Title =new TicketTitle(title);
        UpdatedAt = DateTime.UtcNow;
    }
    public void ChangeDescription(string description)
    {
        Description =new TicketDescription(description);
        UpdatedAt=DateTime.UtcNow;
    }
}
