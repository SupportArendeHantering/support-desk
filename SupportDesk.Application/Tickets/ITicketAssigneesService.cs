
namespace SupportDesk.Application.Tickets
{
    public interface ITicketAssigneesService
    {
        IReadOnlyList<TicketAssignee> GetAllAssignees();
        TicketAssignee? GetAssigneeById(int id);
    }
}