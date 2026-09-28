using SupportDesk.Application.Tickets.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Infrastructure.Tickets.Models;
using System.Text.Json;

namespace SupportDesk.Infrastructure.Tickets;

public class JsonTicketRepository : ITicketRepository
{
    private readonly string _filePath;
    public JsonTicketRepository(string filePath)
    {
        _filePath = filePath;
    }

    public void AddTicket(SupportTicket ticket)
    {
        List<TicketData> tickets;

        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);

            tickets = JsonSerializer.Deserialize<List<TicketData>>(json) ?? [];
        }
        else
            tickets = [];

        TicketData ticketData = new()
        {
            Id = ticket.Id,
            Title = ticket.Title.Value,
            Description = ticket.Description.Value,
            Assignee = ticket.Assignee,
            AssignedCustomerId = ticket.AssignedCustomerId,
            Priority = ticket.Priority,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };

        tickets.Add(ticketData);
        
        string updatedJson = JsonSerializer.Serialize(
            tickets,
            new JsonSerializerOptions
            {
                WriteIndented = true,
            });

        File.WriteAllText(_filePath, updatedJson);
    }
}
