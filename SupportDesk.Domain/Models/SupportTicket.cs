using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Domain.Models;

public class SupportTicket(TicketTitle title, TicketDescription description)
{
    public Guid Id { get; init; }
    public TicketTitle Title { get; set; } = title;
    public TicketDescription Description { get; set; } = description;
    public int? AssigneeId { get; set; }
    public Guid AssignedCustomerId { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Low;
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void ChangeTitle(string title)
    {
        Title = new TicketTitle(title);
        UpdatedAt = DateTime.UtcNow;
    }
    public void ChangeDescription(string description)
    {
        Description = new TicketDescription(description);
        UpdatedAt = DateTime.UtcNow;
    }
    public void ChangePriority(TicketPriority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }
    public void ChangeStatus(TicketStatus status)
    {
        if ((status == TicketStatus.Ongoing || status == TicketStatus.Resolved) && AssigneeId is null)
        {
            throw new InvalidOperationException("An assignee is required before changing the status.");
        }

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignCustomerId(Guid customerId)
    {
        AssignedCustomerId = customerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignAssignee(int assigneeId)
    {
        AssigneeId = assigneeId;
        UpdatedAt = DateTime.UtcNow;
    }
}
