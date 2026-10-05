
namespace SupportDesk.Application.Tickets
{
    public interface ITicketAssigneeService
    {
        IReadOnlyList<TicketAssignee> GetAllAssignees();
        TicketAssignee? GetAssigneeById(int id);
    }
}