namespace SupportDesk.Application.Tickets;

public class TicketAssigneeService : ITicketAssigneeService
{
    private static readonly IReadOnlyList<TicketAssignee> _assignees =
    [
            new (1, "Alice Johnson"),
            new (2, "Bob Smith"),
            new (3, "Charlie Brown"),
            new (4, "David Wilson"),
            new (5, "Eva Davis")
    ];

    public IReadOnlyList<TicketAssignee> GetAllAssignees() => _assignees;

    public TicketAssignee? GetAssigneeById(int id) => _assignees.FirstOrDefault(assignee => assignee.Id == id);
}

