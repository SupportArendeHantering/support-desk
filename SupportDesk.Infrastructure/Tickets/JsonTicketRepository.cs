using SupportDesk.Application.Tickets.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;
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

    public IReadOnlyList<SupportTicket> GetAllTickets()
    {
        if (!File.Exists(_filePath))
            return [];
        string json = File.ReadAllText(_filePath);

        List<TicketData> ticketDataList = JsonSerializer.Deserialize<List<TicketData>>(json) ?? [];

        List<SupportTicket> tickets = [];
        foreach (var ticketData in ticketDataList)
        {
            TicketTitle title = new(ticketData.Title);
            TicketDescription description = new(ticketData.Description);

            SupportTicket ticket = new(title, description)
            {
                Id = ticketData.Id,
                Assignee = ticketData.Assignee,
                AssignedCustomerId = ticketData.AssignedCustomerId,
                Priority = ticketData.Priority,
                Status = ticketData.Status,
                CreatedAt = ticketData.CreatedAt,
                UpdatedAt = ticketData.UpdatedAt
            };
            tickets.Add(ticket);
            
        } 
        return tickets;
    }
    public void UpdateTicket(SupportTicket ticket)
    {
        if (!File.Exists(_filePath))
            throw new InvalidOperationException("Ticket does not exist.");
        string json = File.ReadAllText(_filePath);

        List<TicketData> tickets = JsonSerializer.Deserialize<List<TicketData>>(json)
            ?? throw new InvalidOperationException("Could not load ticket.");
        
        TicketData? existingTicket = tickets.FirstOrDefault(t => t.Id == ticket.Id);

        if (existingTicket == null)
            throw new InvalidOperationException("Ticket not found");

        existingTicket.Title = ticket.Title.Value;
        existingTicket.Description = ticket.Description.Value;
        existingTicket.Assignee = ticket.Assignee;
        existingTicket.AssignedCustomerId = ticket.AssignedCustomerId;
        existingTicket.Priority = ticket.Priority;
        existingTicket.Status = ticket.Status;
        existingTicket.UpdatedAt = ticket.UpdatedAt;

        string updatedJson = JsonSerializer.Serialize(
            tickets,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
        File.WriteAllText(_filePath, updatedJson);
    }
}
