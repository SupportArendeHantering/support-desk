using SupportDesk.Application.Comments.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Infrastructure.Comments.Models;
using SupportDesk.Infrastructure.Settings;
using SupportDesk.Infrastructure.Tickets.Models;
using System.Text.Json;

namespace SupportDesk.Infrastructure.Comments;

public class JsonCommentRepository : ICommentRepository
{
    private readonly string _filePath;
    public JsonCommentRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task AddCommentAsync(TicketComment ticketComment)
    {
        if (!File.Exists(_filePath))
            throw new InvalidOperationException("Ticket does not exist.");

        string json = await File.ReadAllTextAsync(_filePath);

        List<TicketData> tickets =
            JsonSerializer.Deserialize<List<TicketData>>(json,
            JsonSettings.SerializerOptions)
            ?? throw new InvalidOperationException("Could not load tickets.");

        TicketData? existingTicket = 
            tickets.FirstOrDefault(t => t.Id == ticketComment.TicketId);

        if (existingTicket == null)
            throw new InvalidOperationException("Ticket not found.");

        CommentData commentData = new()
        {
            CommentId = ticketComment.CommentId,
            TicketId = ticketComment.TicketId,
            CreatedBy = ticketComment.CreatedBy,
            CommentText = ticketComment.CommentText.Value,
            CreatedAt = ticketComment.CreatedAt
        };


        existingTicket.Comments ??= [];
        existingTicket.Comments.Add(commentData);

        string updatedJson = JsonSerializer.Serialize(
            tickets,
            JsonSettings.SerializerOptions);

        await File.WriteAllTextAsync(_filePath, updatedJson);

        
    }
}
