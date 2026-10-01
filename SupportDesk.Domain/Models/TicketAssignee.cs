namespace SupportDesk.Application.Tickets;

public class TicketAssignee(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
}
