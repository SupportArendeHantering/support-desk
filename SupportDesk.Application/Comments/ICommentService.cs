namespace SupportDesk.Application.Comments
{
    public interface ICommentService
    {
        Task AddCommentAsync(Guid ticketId, Guid assigneeId, string commentText);
    }
}