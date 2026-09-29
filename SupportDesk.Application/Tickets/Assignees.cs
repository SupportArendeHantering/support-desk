namespace SupportDesk.Application.Tickets;

public static class Assignees
{
    public static List<Assignee> GetAssignees()
    {
        return
        [
            new () { Id = 1, Name = "Alice Johnson" },
            new () { Id = 2, Name = "Bob Smith" },
            new () { Id = 3, Name = "Charlie Brown" },
            new () { Id = 4, Name = "David Wilson" },
            new () { Id = 5, Name = "Eva Davis" }
        ];
    }
}
