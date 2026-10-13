using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;
using SupportDesk.Infrastructure.Tickets;

namespace SupportDesk.Test.Infrastructure;

public class JsonTicketRepositoryTests : IDisposable
{
    private readonly string _directory;
    private readonly string _filePath;
    private readonly JsonTicketRepository _repository;

    public JsonTicketRepositoryTests()
    {
        _directory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_directory);

        _filePath = Path.Combine(_directory, "tickets.json");

        _repository = new JsonTicketRepository(_filePath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, true);
    }

    private static SupportTicket CreateTicket(
        string title = "Printer issue",
        string description = "Printer does not work")
    {
        return new SupportTicket(
            new TicketTitle(title),
            new TicketDescription(description))
        {
            Id = Guid.NewGuid(),
            AssignedCustomerId = Guid.NewGuid(),
            AssigneeId = 1,
            Priority = TicketPriority.Normal,
            Status = TicketStatus.New
        };
    }


    [Fact]
    public async Task GetAllTicketsAsync_WhenFileDoesNotExist_ReturnsEmptyList()
    {
        var result = await _repository.GetAllTicketsAsync();

        Assert.Empty(result);
    }


    [Fact]
    public async Task AddTicketAsync_CreatesFile()
    {
        var ticket = CreateTicket();

        await _repository.AddTicketAsync(ticket);

        Assert.True(File.Exists(_filePath));
    }


    [Fact]
    public async Task AddTicketAsync_ThenGetAll_ReturnsAddedTicket()
    {
        var ticket = CreateTicket();

        await _repository.AddTicketAsync(ticket);

        var result = await _repository.GetAllTicketsAsync();

        var addedTicket = Assert.Single(result);

        Assert.Equal(ticket.Id, addedTicket.Id);
        Assert.Equal(ticket.Title.Value, addedTicket.Title.Value);
        Assert.Equal(ticket.Description.Value, addedTicket.Description.Value);
        Assert.Equal(ticket.AssignedCustomerId, addedTicket.AssignedCustomerId);
        Assert.Equal(ticket.AssigneeId, addedTicket.AssigneeId);
        Assert.Equal(ticket.Priority, addedTicket.Priority);
        Assert.Equal(ticket.Status, addedTicket.Status);
    }


    [Fact]
    public async Task AddTicketAsync_MultipleTimes_PreservesAllTickets()
    {
        var first = CreateTicket(
            "Printer issue",
            "Printer does not work");

        var second = CreateTicket(
            "Login issue",
            "User cannot log in");

        await _repository.AddTicketAsync(first);
        await _repository.AddTicketAsync(second);

        var result = await _repository.GetAllTicketsAsync();

        Assert.Equal(2, result.Count);

        Assert.Contains(result, ticket => ticket.Id == first.Id);
        Assert.Contains(result, ticket => ticket.Id == second.Id);
    }


    [Fact]
    public async Task UpdateTicketAsync_ExistingTicket_UpdatesTicket()
    {
        var ticket = CreateTicket();

        await _repository.AddTicketAsync(ticket);

        ticket.ChangeTitle("Updated printer issue");
        ticket.ChangeDescription("Printer is still not working");
        ticket.ChangePriority(TicketPriority.High);

        await _repository.UpdateTicketAsync(ticket);

        var result = await _repository.GetAllTicketsAsync();

        var updatedTicket = Assert.Single(result);

        Assert.Equal("Updated printer issue", updatedTicket.Title.Value);
        Assert.Equal(
            "Printer is still not working",
            updatedTicket.Description.Value);

        Assert.Equal(
            TicketPriority.High,
            updatedTicket.Priority);
    }


    [Fact]
    public async Task UpdateTicketAsync_DoesNotAffectOtherTickets()
    {
        var first = CreateTicket(
            "Printer issue",
            "Printer does not work");

        var second = CreateTicket(
            "Login issue",
            "User cannot log in");

        await _repository.AddTicketAsync(first);
        await _repository.AddTicketAsync(second);

        second.ChangeTitle("Updated login issue");

        await _repository.UpdateTicketAsync(second);

        var result = await _repository.GetAllTicketsAsync();

        var firstTicket =
            result.Single(ticket => ticket.Id == first.Id);

        var secondTicket =
            result.Single(ticket => ticket.Id == second.Id);

        Assert.Equal(
            "Printer issue",
            firstTicket.Title.Value);

        Assert.Equal(
            "Updated login issue",
            secondTicket.Title.Value);
    }


    [Fact]
    public async Task UpdateTicketAsync_WhenFileDoesNotExist_ThrowsInvalidOperationException()
    {
        var ticket = CreateTicket();

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.UpdateTicketAsync(ticket));

        Assert.Equal(
            "Ticket does not exist.",
            exception.Message);
    }


    [Fact]
    public async Task UpdateTicketAsync_UnknownTicket_ThrowsInvalidOperationException()
    {
        var existingTicket = CreateTicket();

        await _repository.AddTicketAsync(existingTicket);

        var unknownTicket = CreateTicket();

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.UpdateTicketAsync(unknownTicket));

        Assert.Equal(
            "Ticket not found.",
            exception.Message);
    }
}